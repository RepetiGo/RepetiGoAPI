using Microsoft.AspNetCore.Authorization;

using RepetiGo.Api.Dtos.UserDtos;

namespace RepetiGo.Api.Controllers
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
        public async Task<ActionResult<ServiceResult<object>>> Register([FromBody] RegisterRequest registerRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ServiceResult<object>.Failure(
                    "Validation failed",
                    HttpStatusCode.BadRequest
                ));
            }

            var result = await _usersService.Register(registerRequest);
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
        public async Task<ActionResult<ServiceResult<UserResponse>>> LogIn([FromBody] LogInRequest logInRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ServiceResult<UserResponse>.Failure(
                    "Validation failed",
                    HttpStatusCode.BadRequest
                ));
            }

            var result = await _usersService.LogIn(logInRequest);
            return result.ToActionResult();
        }

        [AllowAnonymous]
        [HttpPost("refresh/{userId}")]
        public async Task<ActionResult<ServiceResult<UserResponse>>> RefreshToken([FromRoute] string userId, [FromBody] RefreshTokenRequest refreshTokenRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ServiceResult<UserResponse>.Failure(
                    "Validation failed",
                    HttpStatusCode.BadRequest
                ));
            }

            var result = await _usersService.RefreshToken(userId, refreshTokenRequest);
            return result.ToActionResult();
        }

        [AllowAnonymous]
        [HttpPost("reset-password")]
        public async Task<ActionResult<ServiceResult<object>>> ResetPassword([FromBody] ResetPasswordRequest resetPasswordRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ServiceResult<object>.Failure(
                    "Validation failed",
                    HttpStatusCode.BadRequest
                ));
            }
            var result = await _usersService.ResetPassword(resetPasswordRequest);
            return result.ToActionResult();
        }

        [AllowAnonymous]
        [HttpPost("forgot-password")]
        public async Task<ActionResult<ServiceResult<object>>> ForgotPassword([FromBody] ForgotPasswordRequest forgotPasswordRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ServiceResult<object>.Failure(
                    "Validation failed",
                    HttpStatusCode.BadRequest
                ));
            }
            var result = await _usersService.ForgotPassword(forgotPasswordRequest);
            return result.ToActionResult();
        }

        [HttpPost("logout")]
        public async Task<ActionResult<ServiceResult<object>>> LogOut([FromBody] LogOutRequest logOutRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ServiceResult<object>.Failure(
                    "Validation failed",
                    HttpStatusCode.BadRequest
                ));
            }

            var result = await _usersService.LogOut(logOutRequest, User);
            return result.ToActionResult();
        }
    }
}