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
        public async Task<ActionResult<object>> Register([FromBody] RegisterRequestDto registerRequestDto)
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
        [HttpGet("confirm-email")]
        public async Task<ActionResult<object>> ConfirmEmail(string userId, string token)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ServiceResult<object>.Failure(
                    "Validation failed",
                    HttpStatusCode.BadRequest
                ));
            }

            var result = await _usersService.ConfirmEmail(userId, token);
            return result.ToActionResult();
        }

        [AllowAnonymous]
        [HttpGet("resend-confirmation-email/{email}")]
        public async Task<ActionResult<object>> ResendConfirmationEmail([FromRoute] string email)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ServiceResult<object>.Failure(
                    "Validation failed",
                    HttpStatusCode.BadRequest
                ));
            }
            var result = await _usersService.ResendConfirmationEmail(email);
            return result.ToActionResult();
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<UserResponseDto>> LogIn([FromBody] LogInRequestDto logInRequestDto)
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
        [HttpPost("refresh-token/{userId}")]
        public async Task<ActionResult<UserResponseDto>> RefreshToken([FromRoute] string userId, [FromBody] RefreshTokenRequestDto refreshTokenRequestDto)
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

        [Authorize]
        [HttpPost("logout")]
        public async Task<ActionResult<object>> LogOut([FromBody] LogOutRequestDto logOutRequestDto)
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

        [Authorize]
        [HttpGet("profile")]
        public async Task<ActionResult<ProfileResponseDto>> GetProfile()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ServiceResult<ProfileResponseDto>.Failure(
                    "Validation failed",
                    HttpStatusCode.BadRequest
                ));
            }

            var result = await _usersService.GetProfile(User);
            return result.ToActionResult();
        }
    }
}