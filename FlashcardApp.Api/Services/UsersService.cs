using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;

using AutoMapper;

using FlashcardApp.Api.Dtos.ProfileDtos;
using FlashcardApp.Api.Dtos.UserDtos;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Routing;
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
        private readonly IEmailSenderService _emailSenderService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ResponseTemplate _responseTemplate;
        private readonly IUrlHelperFactory _urlHelperFactory;
        private readonly ISettingsService _settingsService;

        public UsersService(IConfiguration configuration,
            IDistributedCache cache,
            UserManager<ApplicationUser> userManager,
            IMapper mapper,
            IEmailSenderService emailSenderService,
            IHttpContextAccessor httpContextAccessor,
            ResponseTemplate responseTemplate,
            IUrlHelperFactory urlHelperFactory,
            ISettingsService settingsService)
        {
            _configuration = configuration;
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetValue<string>("Jwt:Secret") ?? throw new InvalidOperationException("Secret key not found")));
            _cache = cache;
            _userManager = userManager;
            _mapper = mapper;
            _emailSenderService = emailSenderService;
            _httpContextAccessor = httpContextAccessor;
            _responseTemplate = responseTemplate;
            _urlHelperFactory = urlHelperFactory;
            _settingsService = settingsService;
        }
        //Hashtable contains currently active password reset codes
        public static ResetCode resetCode = new ResetCode();

        public async Task<ServiceResult<object>> Register(RegisterRequestDto registerRequestDto)
        {
            var existingUser = await _userManager.FindByEmailAsync(registerRequestDto.Email);
            if (existingUser != null)
            {
                return ServiceResult<object>.Failure(
                    "User with this email already exists",
                    HttpStatusCode.Conflict
                );
            }

            var user = new ApplicationUser
            {
                UserName = registerRequestDto.Email,
                Email = registerRequestDto.Email,
            };

            var result = await _userManager.CreateAsync(user, registerRequestDto.Password);
            if (!result.Succeeded)
            {
                return ServiceResult<object>.Failure(
                    result.Errors.FirstOrDefault()?.Description ?? "User registration failed",
                    HttpStatusCode.BadRequest
                );
            }

            var isSettingsCreated = await _settingsService.CreateUserSettings(user.Id);
            if (!isSettingsCreated)
            {
                await _userManager.DeleteAsync(user); // Clean up user if settings creation fails
                return ServiceResult<object>.Failure(
                    "Failed to create user settings",
                    HttpStatusCode.InternalServerError
                );
            }

            // Send confirmation email
            var isSent = await SendEmailAsync(registerRequestDto.Email, user);
            if (!isSent)
            {
                await _userManager.DeleteAsync(user); // Clean up user if email sending fails
                return ServiceResult<object>.Failure(
                    "Failed to send confirmation email",
                    HttpStatusCode.InternalServerError
                );
            }

            return ServiceResult<object>.Success(
                new { Message = "User registered successfully. Please check your email to confirm your account." },
                HttpStatusCode.Created
                );
        }

        public async Task<ServiceResult<object>> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null)
            {
                return ServiceResult<object>.Success(
                    _responseTemplate.ResendVerificationEmailHtml("/api/users/resend"),
                    HttpStatusCode.BadRequest
                );
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
            {
                return ServiceResult<object>.Success(
                    _responseTemplate.ResendVerificationEmailHtml("/api/users/resend"),
                    HttpStatusCode.BadRequest
                );
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
            {
                return ServiceResult<object>.Success(
                    _responseTemplate.ResendVerificationEmailHtml("/api/users/resend"),
                    HttpStatusCode.BadRequest
                );
            }

            return ServiceResult<object>.Success(
                _responseTemplate.NotificationSuccessHtml(),
                HttpStatusCode.OK
            );
        }

        public async Task<ServiceResult<object>> ResendConfirmationEmail(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null)
            {
                return ServiceResult<object>.Success(
                    _responseTemplate.NotifyNotFoundHtml("/api/users/resend"),
                    HttpStatusCode.NotFound
                );
            }

            if (await _userManager.IsEmailConfirmedAsync(user))
            {
                return ServiceResult<object>.Success(
                    _responseTemplate.NotifyNotFoundHtml("/api/users/resend"),
                    HttpStatusCode.BadRequest
                );
            }

            var isSent = await SendEmailAsync(user.Email!, user);
            if (!isSent)
            {
                return ServiceResult<object>.Success(
                    _responseTemplate.NotifyNotFoundHtml("/api/users/resend"),
                    HttpStatusCode.InternalServerError
                );
            }

            return ServiceResult<object>.Success(
                _responseTemplate.NotifyResendVerificationEmailHtml(),
                HttpStatusCode.OK
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

            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                return ServiceResult<UserResponseDto>.Failure(
                    "Email not confirmed. Please check your email for confirmation link.",
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

        public async Task<ServiceResult<ProfileResponseDto>> UpdateUsername(UpdateUsernameRequestDto updateUsernameRequestDto, ClaimsPrincipal claimsPrincipal)
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

            user.UserName = updateUsernameRequestDto.NewUsername;
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                return ServiceResult<ProfileResponseDto>.Failure(
                    result.Errors.FirstOrDefault()?.Description ?? "Failed to update username",
                    HttpStatusCode.BadRequest
                );
            }

            var profileResponseDto = _mapper.Map<ProfileResponseDto>(user);

            return ServiceResult<ProfileResponseDto>.Success(
                profileResponseDto,
                HttpStatusCode.OK
            );
        }

        private async Task<UserResponseDto> GenerateTokensAsync(ApplicationUser user)
        {
            return new UserResponseDto
            {
                Id = user.Id,
                Email = user.Email!,
                AccessToken = GenerateAccessToken(user),
                RefreshToken = await GenerateRefreshToken(user)
            };
        }

        private async Task<UserResponseDto?> RefreshTokenAsync(ApplicationUser user, string refreshToken)
        {
            var isRevoked = await RevokeRefreshTokenAsync(user, refreshToken);

            return isRevoked ? await GenerateTokensAsync(user) : null;
        }

        private async Task<bool> RevokeRefreshTokenAsync(ApplicationUser user, string refreshToken)
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

        private async Task<bool> SendEmailAsync(string email, ApplicationUser user)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            if (string.IsNullOrEmpty(token))
            {
                return false;
            }

            var confirmationLink = GenerateLink(_httpContextAccessor.HttpContext!, "ConfirmEmail", "Users", new { userId = user.Id, token });

            if (confirmationLink is null)
            {
                return false;
            }

            var safeLink = HtmlEncoder.Default.Encode(confirmationLink);

            var subject = "Confirm your email address";

            await _emailSenderService.SendEmailAsync(email, subject, _responseTemplate.GetEmailVerificationHtml(safeLink), true);
            return true;
        }

        private string? GenerateLink(HttpContext httpContext, string? action, string? controller, object? value)
        {
            var urlHelper = _urlHelperFactory.GetUrlHelper(new ActionContext(httpContext, httpContext.GetRouteData(), new ActionDescriptor()));

            return urlHelper.Action(new UrlActionContext
            {
                Action = action,
                Controller = controller,
                Values = value,
                Protocol = httpContext.Request.Scheme
            });
        }

        public async Task<ServiceResult<object>> ForgotPassword (ForgotPasswordDto forgotPassword)
        {
            var existingUser = await _userManager.FindByEmailAsync(forgotPassword.Email);
            if (existingUser == null)
            {
                return ServiceResult<object>.Failure(
                    "No user exists with this email.",
                    HttpStatusCode.NotFound
                );
            }
            var subject = "Password reset code";
            string code = resetCode.GenerateCode(forgotPassword.Email);
            await _emailSenderService.SendEmailAsync(forgotPassword.Email, subject, _responseTemplate.GetEmailPasswordResetVerificationHtml(code), true);

            return ServiceResult<object>.Success(
                new { Message = "Password reset code sent. Please check your email, including spam folder." },
                HttpStatusCode.OK
                );
        }
        public async Task<ServiceResult<object>> ResetPassword(ResetPasswordDto resetPassword)
        {
            var existingUser = await _userManager.FindByEmailAsync(resetPassword.Email);
            if (existingUser == null)
            {
                return ServiceResult<object>.Failure(
                    "No user exists with this email.",
                    HttpStatusCode.NotFound
                );
            }
            string code = resetPassword.code;
            if (string.IsNullOrEmpty(code) || !resetCode.ValidateResetCode(resetPassword.Email, code))
            {
                return ServiceResult<object>.Failure(
                    "Reset code is wrong.",
                    HttpStatusCode.Unauthorized
                );
            }
            string resetToken;
            try
            {
                resetToken = await _userManager.GeneratePasswordResetTokenAsync(existingUser);
            }
            catch (Exception ex)
            {
                return ServiceResult<object>.Failure(
                    "Failed to generate password reset token due to an internal error.",
                    HttpStatusCode.InternalServerError
                );
            }
            var result = await _userManager.ResetPasswordAsync(existingUser, resetToken, resetPassword.ConfirmPassword);
            
            if (!result.Succeeded)
            {
                var errors = result.Errors.FirstOrDefault();
                return ServiceResult<object>.Failure
                    (
                    errors.Description,
                    HttpStatusCode.BadRequest
                    );
            }
            else
            {
                return ServiceResult<object>.Success(
                new { Message = "Password succesfully changed." },
                HttpStatusCode.OK
                );
            }
        }
    }
}