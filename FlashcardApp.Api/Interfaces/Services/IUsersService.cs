using FlashcardApp.Api.Dtos.ProfileDtos;
using FlashcardApp.Api.Dtos.UserDtos;

namespace FlashcardApp.Api.Interfaces.Services
{
    public interface IUsersService
    {
        Task<ServiceResult<object>> Register(RegisterRequestDto registerRequestDto);
        Task<ServiceResult<UserResponseDto>> LogIn(LogInRequestDto logInRequestDto);
        Task<ServiceResult<UserResponseDto>> RefreshToken(string userId, RefreshTokenRequestDto refreshTokenRequestDto);
        Task<ServiceResult<object>> LogOut(LogOutRequestDto logOutRequestDto, ClaimsPrincipal claimsPrincipal);
        Task<ServiceResult<object>> ConfirmEmail(string userId, string token);
        Task<ServiceResult<object>> ResendConfirmationEmail(string email);
        Task<ServiceResult<object>> ForgotPassword(ForgotPasswordRequestDto forgotPasswordRequestDto);
        Task<ServiceResult<object>> ResetPassword(ResetPasswordRequestDto resetPasswordRequestDto);
        Task<ServiceResult<ProfileResponseDto>> GetProfile(ClaimsPrincipal claimsPrincipal);
        Task<ServiceResult<ProfileResponseDto>> UpdateUsername(UpdateUsernameRequestDto updateUsernameRequestDto, ClaimsPrincipal claimsPrincipal);
        Task<ServiceResult<ProfileResponseDto>> UpdateAvatar(UpdateAvatarRequestDto updateAvatarRequestDto, ClaimsPrincipal claimsPrincipal);
    }
}