using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;

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
        private readonly IEmailSenderService _emailSenderService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly LinkGenerator _linkGenerator;

        public UsersService(IConfiguration configuration, IDistributedCache cache, UserManager<ApplicationUser> userManager, IMapper mapper, IEmailSenderService emailSenderService, IHttpContextAccessor httpContextAccessor, LinkGenerator linkGenerator)
        {
            _configuration = configuration;
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetValue<string>("Jwt:Secret") ?? throw new InvalidOperationException("Secret key not found")));
            _cache = cache;
            _userManager = userManager;
            _mapper = mapper;
            _emailSenderService = emailSenderService;
            _httpContextAccessor = httpContextAccessor;
            _linkGenerator = linkGenerator;
        }

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

            // Send confirmation email
            var isSent = await SendEmailAsync(registerRequestDto.Email, user);
            if (!isSent)
            {
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
                return ServiceResult<object>.Failure(
                    "The link is invalid or has expired. Please request a new one if needed.",
                    HttpStatusCode.BadRequest
                );
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
            {
                return ServiceResult<object>.Failure(
                    "The link is invalid or has expired. Please request a new one if needed.",
                    HttpStatusCode.BadRequest
                );
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
            {
                return ServiceResult<object>.Failure(
                    "The link is invalid or has expired. Please request a new one if needed.",
                    HttpStatusCode.BadRequest
                );
            }

            return ServiceResult<object>.Success(
                new { Message = "Email confirmed successfully. You can now log in." },
                HttpStatusCode.OK
            );
        }

        public async Task<ServiceResult<object>> ResendConfirmationEmail(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null)
            {
                return ServiceResult<object>.Failure(
                    "User not found",
                    HttpStatusCode.NotFound
                );
            }

            if (await _userManager.IsEmailConfirmedAsync(user))
            {
                return ServiceResult<object>.Failure(
                    "Email is already confirmed",
                    HttpStatusCode.BadRequest
                );
            }

            var isSent = await SendEmailAsync(user.Email!, user);
            if (!isSent)
            {
                return ServiceResult<object>.Failure(
                    "Failed to resend confirmation email",
                    HttpStatusCode.InternalServerError
                );
            }

            return ServiceResult<object>.Success(
                new { Message = "Confirmation email resent successfully" },
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

        private async Task<bool> SendEmailAsync(string email, ApplicationUser user)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink = _linkGenerator.GetUriByAction(
                _httpContextAccessor.HttpContext!,
                action: "ConfirmEmail",
                controller: "Users",
                values: new { userId = user.Id, token },
                scheme: _httpContextAccessor?.HttpContext?.Request.Scheme ?? "https");

            if (confirmationLink is null)
            {
                return false;
            }

            var safeLink = HtmlEncoder.Default.Encode(confirmationLink);

            var subject = "Confirm your email address";

            var body = $@"
<!DOCTYPE html>
<html>
<head>

  <meta charset=""utf-8"">
  <meta http-equiv=""x-ua-compatible"" content=""ie=edge"">
  <title>Email Confirmation</title>
  <meta name=""viewport"" content=""width=device-width, initial-scale=1"">
  <style type=""text/css"">
  /**
   * Google webfonts. Recommended to include the .woff version for cross-client compatibility.
   */
  @media screen {{
    @font-face {{
      font-family: 'Source Sans Pro';
      font-style: normal;
      font-weight: 400;
      src: local('Source Sans Pro Regular'), local('SourceSansPro-Regular'), url(https://fonts.gstatic.com/s/sourcesanspro/v10/ODelI1aHBYDBqgeIAH2zlBM0YzuT7MdOe03otPbuUS0.woff) format('woff');
    }}
    @font-face {{
      font-family: 'Source Sans Pro';
      font-style: normal;
      font-weight: 700;
      src: local('Source Sans Pro Bold'), local('SourceSansPro-Bold'), url(https://fonts.gstatic.com/s/sourcesanspro/v10/toadOcfmlt9b38dHJxOBGFkQc6VGVFSmCnC_l7QZG60.woff) format('woff');
    }}
  }}
  /**
   * Avoid browser level font resizing.
   * 1. Windows Mobile
   * 2. iOS / OSX
   */
  body,
  table,
  td,
  a {{
    -ms-text-size-adjust: 100%; /* 1 */
    -webkit-text-size-adjust: 100%; /* 2 */
  }}
  /**
   * Remove extra space added to tables and cells in Outlook.
   */
  table,
  td {{
    mso-table-rspace: 0pt;
    mso-table-lspace: 0pt;
  }}
  /**
   * Better fluid images in Internet Explorer.
   */
  img {{
    -ms-interpolation-mode: bicubic;
  }}
  /**
   * Remove blue links for iOS devices.
   */
  a[x-apple-data-detectors] {{
    font-family: inherit !important;
    font-size: inherit !important;
    font-weight: inherit !important;
    line-height: inherit !important;
    color: inherit !important;
    text-decoration: none !important;
  }}
  /**
   * Fix centering issues in Android 4.4.
   */
  div[style*=""margin: 16px 0;""] {{
    margin: 0 !important;
  }}
  body {{
    width: 100% !important;
    height: 100% !important;
    padding: 0 !important;
    margin: 0 !important;
  }}
  /**
   * Collapse table borders to avoid space between cells.
   */
  table {{
    border-collapse: collapse !important;
  }}
  a {{
    color: #1a82e2;
  }}
  img {{
    height: auto;
    line-height: 100%;
    text-decoration: none;
    border: 0;
    outline: none;
  }}
  </style>

</head>
<body style=""background-color: #e9ecef;"">

  <!-- start preheader -->
  <div class=""preheader"" style=""display: none; max-width: 0; max-height: 0; overflow: hidden; font-size: 1px; line-height: 1px; color: #fff; opacity: 0;"">
    A preheader is the short summary text that follows the subject line when an email is viewed in the inbox.
  </div>
  <!-- end preheader -->

  <!-- start body -->
  <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"">

    <!-- start logo -->
    <tr>
      <td align=""center"" bgcolor=""#e9ecef"">
        <!--[if (gte mso 9)|(IE)]>
        <table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""600"">
        <tr>
        <td align=""center"" valign=""top"" width=""600"">
        <![endif]-->
        <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""max-width: 600px;"">
          <tr>
            <td align=""center"" valign=""top"" style=""padding: 36px 24px;"">
              <a href=""https://zorlen.com"" target=""_blank"" style=""display: inline-block;"">
                <img src=""https://zorlen.com/favicon.ico"" alt=""Logo"" border=""0"" width=""48"" style=""display: block; width: 48px; max-width: 48px; min-width: 48px;"">
              </a>
            </td>
          </tr>
        </table>
        <!--[if (gte mso 9)|(IE)]>
        </td>
        </tr>
        </table>
        <![endif]-->
      </td>
    </tr>
    <!-- end logo -->

    <!-- start hero -->
    <tr>
      <td align=""center"" bgcolor=""#e9ecef"">
        <!--[if (gte mso 9)|(IE)]>
        <table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""600"">
        <tr>
        <td align=""center"" valign=""top"" width=""600"">
        <![endif]-->
        <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""max-width: 600px;"">
          <tr>
            <td align=""left"" bgcolor=""#ffffff"" style=""padding: 36px 24px 0; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; border-top: 3px solid #d4dadf;"">
              <h1 style=""margin: 0; font-size: 32px; font-weight: 700; letter-spacing: -1px; line-height: 48px;"">Confirm Your Email Address</h1>
            </td>
          </tr>
        </table>
        <!--[if (gte mso 9)|(IE)]>
        </td>
        </tr>
        </table>
        <![endif]-->
      </td>
    </tr>
    <!-- end hero -->

    <!-- start copy block -->
    <tr>
      <td align=""center"" bgcolor=""#e9ecef"">
        <!--[if (gte mso 9)|(IE)]>
        <table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""600"">
        <tr>
        <td align=""center"" valign=""top"" width=""600"">
        <![endif]-->
        <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""max-width: 600px;"">

          <!-- start copy -->
          <tr>
            <td align=""left"" bgcolor=""#ffffff"" style=""padding: 24px; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px;"">
              <p style=""margin: 0;"">Tap the button below to confirm your email address. If you didn't create an account with <b>RepetiGo</b>, you can safely delete this email.</p>
            </td>
          </tr>
          <!-- end copy -->

          <!-- start button -->
          <tr>
            <td align=""left"" bgcolor=""#ffffff"">
              <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"">
                <tr>
                  <td align=""center"" bgcolor=""#ffffff"" style=""padding: 12px;"">
                    <table border=""0"" cellpadding=""0"" cellspacing=""0"">
                      <tr>
                        <td align=""center"" bgcolor=""#1a82e2"" style=""border-radius: 6px;"">
                          <a href=""{safeLink}"" target=""_blank"" style=""display: inline-block; padding: 16px 36px; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; color: #ffffff; text-decoration: none; border-radius: 6px;"">Confirm Email</a>
                        </td>
                      </tr>
                    </table>
                  </td>
                </tr>
              </table>
            </td>
          </tr>
          <!-- end button -->

          <!-- start copy -->
          <tr>
            <td align=""left"" bgcolor=""#ffffff"" style=""padding: 24px; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px;"">
              <p style=""margin: 0;"">If that doesn't work, copy and paste the following link in your browser:</p>
              <p style=""margin: 0;""><a href=""{safeLink}"" target=""_blank"">{safeLink}</a></p>
            </td>
          </tr>
          <!-- end copy -->

          <!-- start copy -->
          <tr>
            <td align=""left"" bgcolor=""#ffffff"" style=""padding: 24px; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px; border-bottom: 3px solid #d4dadf"">
              <p style=""margin: 0;"">Best Regards,<br> RepetiGo</p>
            </td>
          </tr>
          <!-- end copy -->

        </table>
        <!--[if (gte mso 9)|(IE)]>
        </td>
        </tr>
        </table>
        <![endif]-->
      </td>
    </tr>
    <!-- end copy block -->

    <!-- start footer -->
    <tr>
      <td align=""center"" bgcolor=""#e9ecef"" style=""padding: 24px;"">
        <!--[if (gte mso 9)|(IE)]>
        <table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""600"">
        <tr>
        <td align=""center"" valign=""top"" width=""600"">
        <![endif]-->
        <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""max-width: 600px;"">

          <!-- start permission -->
          <tr>
            <td align=""center"" bgcolor=""#e9ecef"" style=""padding: 12px 24px; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 14px; line-height: 20px; color: #666;"">
              <p style=""margin: 0;"">You received this email because we received a request for [type_of_action] for your account. If you didn't request [type_of_action] you can safely delete this email.</p>
            </td>
          </tr>
          <!-- end permission -->

          <!-- start unsubscribe -->
          <tr>
            <td align=""center"" bgcolor=""#e9ecef"" style=""padding: 12px 24px; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 14px; line-height: 20px; color: #666;"">
              <p style=""margin: 0;"">To stop receiving these emails, you can <a href=""https://zorlen.com"" target=""_blank"">unsubscribe</a> at any time.</p>
              <p style=""margin: 0;"">If you wish to report this email as spam, please contact our support team.</p>
              <p style=""margin: 0;"">Paste 1234 S. Broadway St. City, State 12345</p>
            </td>
          </tr>
          <!-- end unsubscribe -->

        </table>
        <!--[if (gte mso 9)|(IE)]>
        </td>
        </tr>
        </table>
        <![endif]-->
      </td>
    </tr>
    <!-- end footer -->

  </table>
  <!-- end body -->

</body>
</html>";
            await _emailSenderService.SendEmailAsync(email, subject, body, true);
            return true;
        }
    }
}