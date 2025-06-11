using Microsoft.AspNetCore.Authorization;

namespace backend.Controllers
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
        public async Task<ActionResult<UserResponseDto>> Register([FromBody] RegisterRequestDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ServiceResult<UserResponseDto>.Failure(
                    "Validation failed",
                    HttpStatusCode.BadRequest
                ));
            }

            var result = await _usersService.Register(registerDto);
            return result.ToActionResult();
        }

        [HttpPost("log-in")]
        public async Task<ActionResult<UserResponseDto>> LogIn([FromBody] LogInRequestDto logInDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ServiceResult<UserResponseDto>.Failure(
                    "Validation failed",
                    HttpStatusCode.BadRequest
                ));
            }

            var result = await _usersService.LogIn(logInDto);
            return result.ToActionResult();
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<UserResponseDto>> RefreshToken([FromBody] RefreshTokenRequestDto refreshTokenDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ServiceResult<UserResponseDto>.Failure(
                    "Validation failed",
                    HttpStatusCode.BadRequest
                ));
            }

            var result = await _usersService.RefreshToken(refreshTokenDto);
            return result.ToActionResult();
        }

        [Authorize]
        [HttpPost("log-out")]
        public async Task<ActionResult<object>> LogOut([FromBody] LogOutRequestDto logOutDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ServiceResult<object>.Failure(
                    "Validation failed",
                    HttpStatusCode.BadRequest
                ));
            }

            var result = await _usersService.LogOut(logOutDto, User);
            return result.ToActionResult();
        }

        [Authorize]
        [HttpGet("profile")]
        public async Task<ActionResult<ProfileResponseDto>> GetProfile()
        {
            var result = await _usersService.GetProfile(User);
            return result.ToActionResult();
        }
    }
}