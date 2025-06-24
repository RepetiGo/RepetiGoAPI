using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;

using AutoMapper;

using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using RepetiGo.Api.Dtos.ProfileDtos;
using RepetiGo.Api.Dtos.SettingsDtos;
using RepetiGo.Api.Dtos.UserDtos;

namespace RepetiGo.Api.Services
{
    public class UsersService : IUsersService
    {
        private readonly JwtConfig _jwtConfig;
        private readonly SymmetricSecurityKey _key;
        private readonly IDistributedCache _cache;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly IEmailSenderService _emailSenderService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUrlHelperFactory _urlHelperFactory;
        private readonly ISettingsService _settingsService;
        private readonly IUploadsService _uploadsService;
        private readonly IUnitOfWork _unitOfWork;

        public UsersService(IOptions<JwtConfig> options,
            IDistributedCache cache,
            UserManager<ApplicationUser> userManager,
            IMapper mapper,
            IEmailSenderService emailSenderService,
            IHttpContextAccessor httpContextAccessor,
            IUrlHelperFactory urlHelperFactory,
            ISettingsService settingsService,
            IUploadsService uploadsService,
            IUnitOfWork unitOfWork)
        {
            _jwtConfig = options.Value;
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.Secret));
            _cache = cache;
            _userManager = userManager;
            _mapper = mapper;
            _emailSenderService = emailSenderService;
            _httpContextAccessor = httpContextAccessor;
            _urlHelperFactory = urlHelperFactory;
            _settingsService = settingsService;
            _uploadsService = uploadsService;
            _unitOfWork = unitOfWork;
        }

        public async Task<ServiceResult<object>> Register(RegisterRequest registerRequest)
        {
            var existingUser = await _userManager.FindByEmailAsync(registerRequest.Email);
            if (existingUser != null)
            {
                return ServiceResult<object>.Failure(
                    "User with this email already exists",
                    HttpStatusCode.Conflict
                );
            }

            var user = new ApplicationUser
            {
                UserName = registerRequest.Username,
                Email = registerRequest.Email,
            };

            var result = await _userManager.CreateAsync(user, registerRequest.Password);
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
            var isSent = await SendEmailAsync(registerRequest.Email, user);
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
                    ResponseTemplate.ResendVerificationEmailHtml("/api/users/resend"),
                    HttpStatusCode.BadRequest
                );
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
            {
                return ServiceResult<object>.Success(
                    ResponseTemplate.ResendVerificationEmailHtml("/api/users/resend"),
                    HttpStatusCode.BadRequest
                );
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
            {
                return ServiceResult<object>.Success(
                    ResponseTemplate.ResendVerificationEmailHtml("/api/users/resend"),
                    HttpStatusCode.BadRequest
                );
            }

            return ServiceResult<object>.Success(
                ResponseTemplate.NotificationSuccessHtml(),
                HttpStatusCode.OK
            );
        }

        public async Task<ServiceResult<object>> ResendConfirmationEmail(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null)
            {
                return ServiceResult<object>.Success(
                    ResponseTemplate.NotifyNotFoundHtml("/api/users/resend"),
                    HttpStatusCode.NotFound
                );
            }

            if (await _userManager.IsEmailConfirmedAsync(user))
            {
                return ServiceResult<object>.Success(
                    ResponseTemplate.NotifyNotFoundHtml("/api/users/resend"),
                    HttpStatusCode.BadRequest
                );
            }

            var isSent = await SendEmailAsync(user.Email!, user);
            if (!isSent)
            {
                return ServiceResult<object>.Success(
                    ResponseTemplate.NotifyNotFoundHtml("/api/users/resend"),
                    HttpStatusCode.InternalServerError
                );
            }

            return ServiceResult<object>.Success(
                ResponseTemplate.NotifyResendVerificationEmailHtml(),
                HttpStatusCode.OK
            );
        }

        public async Task<ServiceResult<UserResponse>> LogIn(LogInRequest logInRequest)
        {
            var user = await _userManager.FindByEmailAsync(logInRequest.Email);
            if (user is null)
            {
                return ServiceResult<UserResponse>.Failure(
                    "User not found",
                    HttpStatusCode.Unauthorized
                );
            }

            if (await _userManager.IsLockedOutAsync(user))
            {
                var lockoutEnd = await _userManager.GetLockoutEndDateAsync(user);
                var remainingLockoutTime = lockoutEnd?.Subtract(DateTime.UtcNow);

                return ServiceResult<UserResponse>.Failure(
                    $"Account is locked. Please try again in {remainingLockoutTime?.TotalMinutes} minutes.",
                    HttpStatusCode.Locked
                );
            }

            var isCorrect = await _userManager.CheckPasswordAsync(user, logInRequest.Password);
            if (!isCorrect)
            {
                await _userManager.AccessFailedAsync(user);

                if (await _userManager.IsLockedOutAsync(user))
                {
                    var lockoutEnd = await _userManager.GetLockoutEndDateAsync(user);
                    var remainingLockoutTime = lockoutEnd?.Subtract(DateTime.UtcNow);
                    return ServiceResult<UserResponse>.Failure(
                        $"Too many failed attempts. Please try again in {remainingLockoutTime?.TotalMinutes} minutes.",
                        HttpStatusCode.Locked
                    );
                }

                var failedAttempts = await _userManager.GetAccessFailedCountAsync(user);
                var maxAttempts = _userManager.Options.Lockout.MaxFailedAccessAttempts;
                var remainingAttempts = maxAttempts - failedAttempts;

                return ServiceResult<UserResponse>.Failure(
                    $"Incorrect password. {remainingAttempts} attempts remaining.",
                    HttpStatusCode.Unauthorized
                );
            }

            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                return ServiceResult<UserResponse>.Failure(
                    "Email not confirmed. Please check your email for confirmation link.",
                    HttpStatusCode.Unauthorized
                );
            }

            await _userManager.ResetAccessFailedCountAsync(user);

            return ServiceResult<UserResponse>.Success(
                await GenerateTokensAsync(user),
                HttpStatusCode.OK
            );
        }

        public async Task<ServiceResult<UserResponse>> RefreshToken(string userId, RefreshTokenRequest refreshTokenRequest)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
            {
                return ServiceResult<UserResponse>.Failure(
                    "User not found",
                    HttpStatusCode.BadRequest
                );
            }

            var tokens = await RefreshTokenAsync(user, refreshTokenRequest.RefreshToken);
            if (tokens is null)
            {
                return ServiceResult<UserResponse>.Failure(
                    "Invalid refresh token",
                    HttpStatusCode.BadRequest
                );
            }

            return ServiceResult<UserResponse>.Success(
                tokens,
                HttpStatusCode.OK
            );
        }

        public async Task<ServiceResult<object>> LogOut(LogOutRequest logOutRequest, ClaimsPrincipal claimsPrincipal)
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

            var result = await RevokeRefreshTokenAsync(user, logOutRequest.RefreshToken);
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

        public async Task<ServiceResult<ProfileResponse>> GetProfile(ClaimsPrincipal claimsPrincipal)
        {
            var userId = claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return ServiceResult<ProfileResponse>.Failure(
                    "User not authenticated",
                    HttpStatusCode.Unauthorized
                );
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
            {
                return ServiceResult<ProfileResponse>.Failure(
                    "User not found",
                    HttpStatusCode.NotFound
                );
            }

            var profileResponse = _mapper.Map<ProfileResponse>(user);

            return ServiceResult<ProfileResponse>.Success(
                profileResponse,
                HttpStatusCode.OK
            );
        }

        public async Task<ServiceResult<ProfileResponse>> UpdateUsername(UpdateUsernameRequest updateUsernameRequest, ClaimsPrincipal claimsPrincipal)
        {
            var userId = claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return ServiceResult<ProfileResponse>.Failure(
                    "User not authenticated",
                    HttpStatusCode.Unauthorized
                );
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
            {
                return ServiceResult<ProfileResponse>.Failure(
                    "User not found",
                    HttpStatusCode.NotFound
                );
            }

            user.UserName = updateUsernameRequest.NewUsername;
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                return ServiceResult<ProfileResponse>.Failure(
                    result.Errors.FirstOrDefault()?.Description ?? "Failed to update username",
                    HttpStatusCode.BadRequest
                );
            }

            var profileResponse = _mapper.Map<ProfileResponse>(user);

            return ServiceResult<ProfileResponse>.Success(
                profileResponse,
                HttpStatusCode.OK
            );
        }

        private async Task<UserResponse> GenerateTokensAsync(ApplicationUser user)
        {
            return new UserResponse
            {
                Id = user.Id,
                Email = user.Email!,
                AccessToken = GenerateAccessToken(user),
                RefreshToken = await GenerateRefreshToken(user)
            };
        }

        private async Task<UserResponse?> RefreshTokenAsync(ApplicationUser user, string refreshToken)
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
                Issuer = _jwtConfig.Issuer,
                Audience = _jwtConfig.Audience,
                Expires = DateTime.UtcNow.AddMinutes(_jwtConfig.TokenValidityInMinutes),
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
                AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(_jwtConfig.RefreshTokenValidityInDays)
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

            await _emailSenderService.SendEmailAsync(email, subject, ResponseTemplate.GetEmailVerificationHtml(safeLink), true);
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

        public async Task<ServiceResult<object>> ForgotPassword(ForgotPasswordRequest forgotPasswordRequest)
        {
            var existingUser = await _userManager.FindByEmailAsync(forgotPasswordRequest.Email);
            if (existingUser == null)
            {
                return ServiceResult<object>.Failure(
                    "No user exists with this email.",
                    HttpStatusCode.NotFound
                );
            }
            var subject = "Password reset code";
            string code = ResetCode.GenerateCode(forgotPasswordRequest.Email);
            await _emailSenderService.SendEmailAsync(forgotPasswordRequest.Email, subject, ResponseTemplate.GetEmailPasswordResetVerificationHtml(code), true);

            return ServiceResult<object>.Success(
                new { Message = "Password reset code sent. Please check your email, including spam folder." },
                HttpStatusCode.OK
                );
        }

        public async Task<ServiceResult<object>> ResetPassword(ResetPasswordRequest resetPasswordRequest)
        {
            var existingUser = await _userManager.FindByEmailAsync(resetPasswordRequest.Email);
            if (existingUser == null)
            {
                return ServiceResult<object>.Failure(
                    "No user exists with this email.",
                    HttpStatusCode.NotFound
                );
            }
            string code = resetPasswordRequest.Code;
            if (string.IsNullOrEmpty(code) || !ResetCode.ValidateResetCode(resetPasswordRequest.Email, code))
            {
                return ServiceResult<object>.Failure(
                    "Reset code is wrong or has expired.",
                    HttpStatusCode.Unauthorized
                );
            }
            string resetToken;
            try
            {
                resetToken = await _userManager.GeneratePasswordResetTokenAsync(existingUser);
            }
            catch (Exception)
            {
                return ServiceResult<object>.Failure(
                    "Failed to generate password reset token due to an internal error.",
                    HttpStatusCode.InternalServerError
                );
            }
            var result = await _userManager.ResetPasswordAsync(existingUser, resetToken, resetPasswordRequest.Password);

            if (!result.Succeeded)
            {
                var errors = result.Errors.FirstOrDefault();
                return ServiceResult<object>.Failure
                    (
                    errors!.Description,
                    HttpStatusCode.BadRequest
                    );
            }

            return ServiceResult<object>.Success(
            new { Message = "Password successfully changed." },
            HttpStatusCode.OK
            );

        }

        public async Task<ServiceResult<ProfileResponse>> UpdateAvatar(UpdateAvatarRequest updateAvatarRequest, ClaimsPrincipal claimsPrincipal)
        {
            var userId = claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return ServiceResult<ProfileResponse>.Failure(
                    "User not authenticated",
                    HttpStatusCode.Unauthorized
                );
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
            {
                return ServiceResult<ProfileResponse>.Failure(
                    "User not found",
                    HttpStatusCode.NotFound
                );
            }

            if (updateAvatarRequest.File == null || updateAvatarRequest.File.Length == 0)
            {
                return ServiceResult<ProfileResponse>.Failure(
                    "Avatar file is required",
                    HttpStatusCode.BadRequest
                );
            }

            var uploadResult = await _uploadsService.UploadImageAsync(updateAvatarRequest.File);
            if (!uploadResult.IsSuccess)
            {
                return ServiceResult<ProfileResponse>.Failure(
                    uploadResult.ErrorMessage ?? "Failed to upload avatar",
                    HttpStatusCode.InternalServerError
                );
            }

            // Delete the old image if it exists
            var oldAvatarPublicId = user.AvatarPublicId;
            if (!string.IsNullOrEmpty(oldAvatarPublicId))
            {
                var deleteResult = await _uploadsService.DeleteImageAsync(oldAvatarPublicId);
                if (!deleteResult.IsSuccess)
                {
                    return ServiceResult<ProfileResponse>.Failure(
                        deleteResult.ErrorMessage ?? "Failed to delete old avatar",
                        HttpStatusCode.InternalServerError
                    );
                }
            }

            user.AvatarUrl = uploadResult.SecureUrl;
            user.AvatarPublicId = uploadResult.PublicId;

            // Update user with new avatar URL and public ID
            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded && uploadResult.PublicId is not null)
            {
                // Delete the newly uploaded image since we're rolling back
                await _uploadsService.DeleteImageAsync(uploadResult.PublicId);
                return ServiceResult<ProfileResponse>.Failure(
                    updateResult.Errors.FirstOrDefault()?.Description ?? "Failed to update avatar",
                    HttpStatusCode.BadRequest
                );
            }

            var profileResponse = _mapper.Map<ProfileResponse>(user);

            return ServiceResult<ProfileResponse>.Success(
                profileResponse,
                HttpStatusCode.OK
            );
        }

        public async Task<ServiceResult<SettingsResponse>> UpdateSettings(UpdateSettingsRequest updateSettingsRequest, ClaimsPrincipal claimsPrincipal)
        {
            var userId = claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return ServiceResult<SettingsResponse>.Failure(
                    "User not authenticated",
                    HttpStatusCode.Unauthorized
                );
            }

            var settings = await _unitOfWork.SettingsRepository.GetSettingsByUserIdAsync(userId);
            if (settings == null)
            {
                return ServiceResult<SettingsResponse>.Failure(
                    "Settings not found.",
                    HttpStatusCode.NotFound
                );
            }

            _mapper.Map(updateSettingsRequest, settings);
            await _unitOfWork.SettingsRepository.UpdateAsync(settings);
            await _unitOfWork.SaveAsync();

            var settingsResponse = _mapper.Map<SettingsResponse>(settings);
            return ServiceResult<SettingsResponse>.Success(
                settingsResponse,
                HttpStatusCode.OK
            );
        }
    }
}