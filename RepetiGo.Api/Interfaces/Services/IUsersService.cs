using RepetiGo.Api.Dtos.ProfileDtos;
using RepetiGo.Api.Dtos.UserDtos;

namespace RepetiGo.Api.Interfaces.Services
{
    public interface IUsersService
    {
        Task<ServiceResult<object>> Register(RegisterRequest registerRequest);
        Task<ServiceResult<UserResponse>> LogIn(LogInRequest logInRequest);
        Task<ServiceResult<UserResponse>> RefreshToken(string userId, RefreshTokenRequest refreshTokenRequest);
        Task<ServiceResult<object>> LogOut(LogOutRequest logOutRequest, ClaimsPrincipal claimsPrincipal);
        Task<ServiceResult<object>> ConfirmEmail(string userId, string token);
        Task<ServiceResult<object>> ResendConfirmationEmail(string email);
        Task<ServiceResult<object>> ForgotPassword(ForgotPasswordRequest forgotPasswordRequest);
        Task<ServiceResult<object>> ResetPassword(ResetPasswordRequest resetPasswordRequest);
        Task<ServiceResult<ProfileResponse>> GetProfile(ClaimsPrincipal claimsPrincipal);
        Task<ServiceResult<ProfileResponse>> UpdateUsername(UpdateUsernameRequest updateUsernameRequest, ClaimsPrincipal claimsPrincipal);
        Task<ServiceResult<ProfileResponse>> UpdateAvatar(UpdateAvatarRequest updateAvatarRequest, ClaimsPrincipal claimsPrincipal);
    }
}