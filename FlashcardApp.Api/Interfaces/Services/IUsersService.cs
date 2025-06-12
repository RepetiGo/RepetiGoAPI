using FlashcardApp.Api.Dtos.UserDtos;

namespace FlashcardApp.Api.Interfaces.Services
{
    public interface IUsersService
    {
        // User management
        Task<ServiceResult<object>> Register(RegisterRequestDto registerRequestDto);
        Task<ServiceResult<UserResponseDto>> LogIn(LogInRequestDto logInRequestDto);
        Task<ServiceResult<UserResponseDto>> RefreshToken(string userId, RefreshTokenRequestDto refreshTokenRequestDto);
        Task<ServiceResult<object>> LogOut(LogOutRequestDto logOutRequestDto, ClaimsPrincipal claimsPrincipal);
        Task<ServiceResult<ProfileResponseDto>> GetProfile(ClaimsPrincipal claimsPrincipal);
        Task<ServiceResult<object>> ConfirmEmail(string userId, string token);
        Task<ServiceResult<object>> ResendConfirmationEmail(string email);

        // Token management
        Task<UserResponseDto> GenerateTokensAsync(ApplicationUser user);
        Task<UserResponseDto?> RefreshTokenAsync(ApplicationUser user, string refreshToken);
        Task<bool> RevokeRefreshTokenAsync(ApplicationUser user, string refreshToken);
    }
}