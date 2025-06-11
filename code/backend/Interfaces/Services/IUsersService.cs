using backend.Dtos.UserDtos;

namespace backend.Interfaces.Services
{
    public interface IUsersService
    {
        // User management
        Task<ServiceResult<UserResponseDto>> Register(RegisterRequestDto registerDto);
        Task<ServiceResult<UserResponseDto>> LogIn(LogInRequestDto logInDto);
        Task<ServiceResult<UserResponseDto>> RefreshToken(RefreshTokenRequestDto refreshTokenDto);
        Task<ServiceResult<object>> LogOut(LogOutRequestDto logOutDto, ClaimsPrincipal claimsPrincipal);
        Task<ServiceResult<ProfileResponseDto>> GetProfile(ClaimsPrincipal claimsPrincipal);

        // Token management
        Task<UserResponseDto> GenerateTokensAsync(ApplicationUser user);
        Task<UserResponseDto?> RefreshTokenAsync(ApplicationUser user, string refreshToken);
        Task<bool> RevokeRefreshTokenAsync(ApplicationUser user, string refreshToken);
    }
}