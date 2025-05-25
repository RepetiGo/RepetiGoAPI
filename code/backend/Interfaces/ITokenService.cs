namespace backend.Interfaces
{
    public interface ITokenService
    {
        Task<AuthResponseDto> GenerateTokensAsync(IdentityUser user);
        Task<AuthResponseDto?> RefreshTokenAsync(IdentityUser user, string refreshToken);
        Task<bool> RevokeRefreshTokenAsync(IdentityUser user, string refreshToken);
    }
}
