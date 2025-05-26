using Microsoft.Extensions.Caching.Distributed;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;

namespace backend.Services
{
    public class TokenService : ITokenService
    {
        private readonly SymmetricSecurityKey _key;
        private readonly IDistributedCache _cache;
        private readonly IConfiguration _configuration;

        public TokenService(IConfiguration configuration, IDistributedCache cache)
        {
            _configuration = configuration;
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetValue<string>("Jwt:Secret") ?? throw new InvalidOperationException("Secret key not found")));
            _cache = cache;
        }

        public async Task<AuthenticatedResponseDto> GenerateTokensAsync(ApplicationUser user)
        {
            return new AuthenticatedResponseDto
            {
                UserId = user.Id,
                Email = user.Email!,
                AccessToken = GenerateAccessToken(user),
                RefreshToken = await GenerateRefreshToken(user)
            };
        }

        public async Task<AuthenticatedResponseDto?> RefreshTokenAsync(ApplicationUser user, string refreshToken)
        {
            var cachedUserId = await _cache.GetStringAsync(refreshToken);
            if (string.IsNullOrEmpty(cachedUserId) || cachedUserId != user.Id)
            {
                return null;
            }

            await _cache.RemoveAsync(refreshToken);
            return await GenerateTokensAsync(user);
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
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
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
            var refreshToken = Convert.ToBase64String(randomNumber);
            await _cache.SetStringAsync(refreshToken, user.Id, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(_configuration.GetValue<int>("Jwt:RefreshTokenValidityInDays"))
            });
            return refreshToken;
        }
    }
}
