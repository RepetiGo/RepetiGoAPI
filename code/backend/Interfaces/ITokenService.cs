namespace backend.Interfaces
{
    public interface ITokenService
    {
        Task<AuthenticatedResponseDto> GenerateTokensAsync(ApplicationUser user);
        Task<AuthenticatedResponseDto?> RefreshTokenAsync(ApplicationUser user, string refreshToken);
        Task<bool> RevokeRefreshTokenAsync(ApplicationUser user, string refreshToken);
    }
}