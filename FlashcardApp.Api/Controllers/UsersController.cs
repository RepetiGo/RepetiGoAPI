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

        [HttpPost("register")]
        public async Task<ActionResult<UserResponseDto>> Register([FromBody] RegisterRequestDto registerRequestDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ServiceResult<UserResponseDto>.Failure(
                    "Validation failed",
                    HttpStatusCode.BadRequest
                ));
            }

            var result = await _usersService.Register(registerRequestDto);
            return result.ToActionResult();
        }

        [HttpPost("log-in")]
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
        [HttpPost("log-out")]
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