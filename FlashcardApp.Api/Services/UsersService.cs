using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;

using AutoMapper;

using FlashcardApp.Api.Dtos.UserDtos;

using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.IdentityModel.Tokens;

namespace FlashcardApp.Api.Services
{
    public class UsersService : IUsersService
    {
        private readonly SymmetricSecurityKey _key;
        private readonly IDistributedCache _cache;
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public UsersService(IConfiguration configuration, IDistributedCache cache, UserManager<ApplicationUser> userManager, IMapper mapper)
        {
            _configuration = configuration;
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetValue<string>("Jwt:Secret") ?? throw new InvalidOperationException("Secret key not found")));
            _cache = cache;
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<ServiceResult<UserResponseDto>> Register(RegisterRequestDto registerRequestDto)
        {
            var existingUser = await _userManager.FindByEmailAsync(registerRequestDto.Email);
            if (existingUser != null)
            {
                return ServiceResult<UserResponseDto>.Failure(
                    "User with this email already exists",
                    HttpStatusCode.Conflict
                );
            }

            var user = new ApplicationUser
            {
                UserName = registerRequestDto.Email,
                Email = registerRequestDto.Email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, registerRequestDto.Password);
            if (!result.Succeeded)
            {
                return ServiceResult<UserResponseDto>.Failure(
                    result.Errors.FirstOrDefault()?.Description ?? "User registration failed",
                    HttpStatusCode.BadRequest
                );
            }

            return ServiceResult<UserResponseDto>.Success(
                await GenerateTokensAsync(user),
                HttpStatusCode.Created
            );
        }

        public async Task<ServiceResult<UserResponseDto>> LogIn(LogInRequestDto logInRequestDto)
        {
            var user = await _userManager.FindByEmailAsync(logInRequestDto.Email);
            if (user is null)
            {
                return ServiceResult<UserResponseDto>.Failure(
                    "User not found",
                    HttpStatusCode.Unauthorized
                );
            }

            var isCorrect = await _userManager.CheckPasswordAsync(user, logInRequestDto.Password);
            if (!isCorrect)
            {
                return ServiceResult<UserResponseDto>.Failure(
                    "Incorrect password",
                    HttpStatusCode.Unauthorized
                );
            }

            return ServiceResult<UserResponseDto>.Success(
                await GenerateTokensAsync(user),
                HttpStatusCode.OK
            );
        }

        public async Task<ServiceResult<UserResponseDto>> RefreshToken(string userId, RefreshTokenRequestDto refreshTokenRequestDto)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
            {
                return ServiceResult<UserResponseDto>.Failure(
                    "User not found",
                    HttpStatusCode.BadRequest
                );
            }

            var tokens = await RefreshTokenAsync(user, refreshTokenRequestDto.RefreshToken);
            if (tokens is null)
            {
                return ServiceResult<UserResponseDto>.Failure(
                    "Invalid refresh token",
                    HttpStatusCode.BadRequest
                );
            }

            return ServiceResult<UserResponseDto>.Success(
                tokens,
                HttpStatusCode.OK
            );
        }

        public async Task<ServiceResult<object>> LogOut(LogOutRequestDto logOutRequestDto, ClaimsPrincipal claimsPrincipal)
        {
            var userId = claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return ServiceResult<object>.Failure(
                    "User not authenticated",
                    HttpStatusCode.Unauthorized
                );
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
            {
                return ServiceResult<object>.Failure(
                    "User not found",
                    HttpStatusCode.NotFound
                );
            }

            var result = await RevokeRefreshTokenAsync(user, logOutRequestDto.RefreshToken);
            if (!result)
            {
                return ServiceResult<object>.Failure(
                    "Failed to revoke refresh token",
                    HttpStatusCode.BadRequest
                );
            }

            return ServiceResult<object>.Success(
                new { Message = "User logged out successfully" },
                HttpStatusCode.OK
            );
        }

        public async Task<ServiceResult<ProfileResponseDto>> GetProfile(ClaimsPrincipal claimsPrincipal)
        {
            var userId = claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return ServiceResult<ProfileResponseDto>.Failure(
                    "User not authenticated",
                    HttpStatusCode.Unauthorized
                );
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
            {
                return ServiceResult<ProfileResponseDto>.Failure(
                    "User not found",
                    HttpStatusCode.NotFound
                );
            }

            var profileResponseDto = _mapper.Map<ProfileResponseDto>(user);

            return ServiceResult<ProfileResponseDto>.Success(
                profileResponseDto,
                HttpStatusCode.OK
            );
        }

        public async Task<UserResponseDto> GenerateTokensAsync(ApplicationUser user)
        {
            return new UserResponseDto
            {
                Id = user.Id,
                Email = user.Email!,
                AccessToken = GenerateAccessToken(user),
                RefreshToken = await GenerateRefreshToken(user)
            };
        }

        public async Task<UserResponseDto?> RefreshTokenAsync(ApplicationUser user, string refreshToken)
        {
            var isRevoked = await RevokeRefreshTokenAsync(user, refreshToken);

            return isRevoked ? await GenerateTokensAsync(user) : null;
        }

        public async Task<bool> RevokeRefreshTokenAsync(ApplicationUser user, string refreshToken)
        {
            var cachedUserId = await _cache.GetStringAsync(refreshToken);
            if (string.IsNullOrEmpty(cachedUserId) || cachedUserId != user.Id)
            {
                return false;
            }
            await _cache.RemoveAsync(refreshToken);
            return true;
        }

        private string GenerateAccessToken(ApplicationUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, user.Id)
            };

            var credentials = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Issuer = _configuration.GetValue<string>("Jwt:Issuer") ?? throw new InvalidOperationException("Issuer not found"),
                Audience = _configuration.GetValue<string>("Jwt:Audience") ?? throw new InvalidOperationException("Audience not found"),
                Expires = DateTime.UtcNow.AddMinutes(_configuration.GetValue<double?>("Jwt:TokenValidityInMinutes") ?? 15),
                SigningCredentials = credentials,
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private async Task<string> GenerateRefreshToken(ApplicationUser user)
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            var refreshToken = WebEncoders.Base64UrlEncode(randomNumber);
            await _cache.SetStringAsync(refreshToken, user.Id, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(_configuration.GetValue<int>("Jwt:RefreshTokenValidityInDays"))
            });
            return refreshToken;
        }
    }
}