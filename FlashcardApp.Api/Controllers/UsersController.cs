using FlashcardApp.Api.Dtos.UserDtos;

using Microsoft.AspNetCore.Authorization;

namespace FlashcardApp.Api.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUsersService _usersService;

        public UsersController(IUsersService tokenService)
        {
            _usersService = tokenService;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult<ServiceResult<object>>> Register([FromBody] RegisterRequestDto registerRequestDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ServiceResult<object>.Failure(
                    "Validation failed",
                    HttpStatusCode.BadRequest
                ));
            }

            var result = await _usersService.Register(registerRequestDto);
            return result.ToActionResult();
        }

        [AllowAnonymous]
        [HttpGet("confirm")]
        public async Task<ActionResult<ServiceResult<object>>> ConfirmEmail(string userId, string token)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ServiceResult<object>.Failure(
                    "Validation failed",
                    HttpStatusCode.BadRequest
                ));
            }

            var result = await _usersService.ConfirmEmail(userId, token);
            if (result.Data is null)
            {
                return BadRequest(ServiceResult<object>.Failure(
                    result.ErrorMessage ?? "Email confirmation failed",
                    result.StatusCode
                ));
            }
            return Content(result.Data.ToString() ?? string.Empty, "text/html");
        }

        [AllowAnonymous]
        [HttpGet("resend")]
        public async Task<ActionResult<ServiceResult<object>>> ResendConfirmationEmail([FromQuery] string email)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ServiceResult<object>.Failure(
                    "Validation failed",
                    HttpStatusCode.BadRequest
                ));
            }
            var result = await _usersService.ResendConfirmationEmail(email);
            if (result.Data is null)
            {
                return BadRequest(ServiceResult<object>.Failure(
                    result.ErrorMessage ?? "Resend confirmation email failed",
                    result.StatusCode
                ));
            }
            return Content(result.Data.ToString() ?? string.Empty, "text/html");
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<ServiceResult<UserResponseDto>>> LogIn([FromBody] LogInRequestDto logInRequestDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ServiceResult<UserResponseDto>.Failure(
                    "Validation failed",
                    HttpStatusCode.BadRequest
                ));
            }

            var result = await _usersService.LogIn(logInRequestDto);
            return result.ToActionResult();
        }

        [AllowAnonymous]
        [HttpPost("refresh/{userId}")]
        public async Task<ActionResult<ServiceResult<UserResponseDto>>> RefreshToken([FromRoute] string userId, [FromBody] RefreshTokenRequestDto refreshTokenRequestDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ServiceResult<UserResponseDto>.Failure(
                    "Validation failed",
                    HttpStatusCode.BadRequest
                ));
            }

            var result = await _usersService.RefreshToken(userId, refreshTokenRequestDto);
            return result.ToActionResult();
        }

        [AllowAnonymous]
        [HttpPost("reset-password")]
        public async Task<ActionResult<object>> ResetPassword([FromBody] ResetPasswordDto resetPasswordRequestDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ServiceResult<object>.Failure(
                    "Validation failed",
                    HttpStatusCode.BadRequest
                ));
            }
            var result = await _usersService.ResetPassword(resetPasswordRequestDto);
            return result;
        }

        [AllowAnonymous]
        [HttpPost("forgot-password")]
        public async Task<ActionResult<object>> ForgotPassword([FromBody] ForgotPasswordDto forgotPassword )
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ServiceResult<object>.Failure(
                    "Validation failed",
                    HttpStatusCode.BadRequest
                ));
            }
            var result = await _usersService.ForgotPassword(forgotPassword);
            return result;
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<ActionResult<ServiceResult<object>>> LogOut([FromBody] LogOutRequestDto logOutRequestDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ServiceResult<object>.Failure(
                    "Validation failed",
                    HttpStatusCode.BadRequest
                ));
            }

            var result = await _usersService.LogOut(logOutRequestDto, User);
            return result.ToActionResult();
        }
    }
}