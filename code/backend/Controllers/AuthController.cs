using AutoMapper;

using Microsoft.AspNetCore.Authorization;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;

        public AuthController(UserManager<ApplicationUser> userManager, ITokenService tokenService, IMapper mapper)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _mapper = mapper;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthenticatedResponseDto>> Register([FromBody] RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);
            if (existingUser != null)
            {
                return Conflict(new ResponseErrorDto
                {
                    StatusCode = HttpStatusCode.Conflict,
                    Message = "User with this email already exists",
                    Error = "Conflict"
                });
            }

            var user = new ApplicationUser
            {
                UserName = registerDto.Email,
                Email = registerDto.Email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
            {
                return BadRequest(new ResponseErrorDto
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Message = result.Errors.FirstOrDefault()?.Description ?? "User registration failed",
                    Error = "Bad Request"
                });
            }

            return Ok(await _tokenService.GenerateTokensAsync(user));
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthenticatedResponseDto>> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user is null)
            {
                return Unauthorized(new ResponseErrorDto
                {
                    StatusCode = HttpStatusCode.Unauthorized,
                    Message = "User not found",
                    Error = "Unauthorized"
                });
            }

            var isValid = await _userManager.CheckPasswordAsync(user, loginDto.Password);
            if (!isValid)
            {
                return Unauthorized(new ResponseErrorDto
                {
                    StatusCode = HttpStatusCode.Unauthorized,
                    Message = "Password is incorrect",
                    Error = "Unauthorized"
                });
            }

            return Ok(await _tokenService.GenerateTokensAsync(user));
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<AuthenticatedResponseDto>> RefreshToken([FromBody] RefreshTokenDto refreshTokenDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByIdAsync(refreshTokenDto.Id);
            if (user is null)
            {
                return Unauthorized(new ResponseErrorDto
                {
                    StatusCode = HttpStatusCode.Unauthorized,
                    Message = "User not found",
                    Error = "Unauthorized"
                });
            }

            var tokens = await _tokenService.RefreshTokenAsync(user, refreshTokenDto.RefreshToken);
            if (tokens is null)
            {
                return Unauthorized(new ResponseErrorDto
                {
                    StatusCode = HttpStatusCode.Unauthorized,
                    Message = "Invalid refresh token",
                    Error = "Unauthorized"
                });
            }

            return Ok(tokens);
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] LogoutDTo logoutDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new ResponseErrorDto
                {
                    StatusCode = HttpStatusCode.Unauthorized,
                    Message = "User not authenticated",
                    Error = "Unauthorized"
                });
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
            {
                return BadRequest(new ResponseErrorDto
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Message = "User not found",
                    Error = "Bad Request"
                });
            }

            var result = await _tokenService.RevokeRefreshTokenAsync(user, logoutDto.RefreshToken);
            if (!result)
            {
                return BadRequest(new ResponseErrorDto
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Message = "Failed to revoke refresh token",
                    Error = "Bad Request"
                });
            }

            return Ok(new { Message = "Logged out successfully" });
        }

        [Authorize]
        [HttpGet("profile")]
        public async Task<ActionResult<ApplicationUser>> GetProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new ResponseErrorDto
                {
                    StatusCode = HttpStatusCode.Unauthorized,
                    Message = "User not authenticated",
                    Error = "Unauthorized"
                });
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
            {
                return NotFound(new ResponseErrorDto
                {
                    StatusCode = HttpStatusCode.NotFound,
                    Message = "User not found",
                    Error = "Not Found"
                });
            }

            var userDto = _mapper.Map<ProfileDto>(user);

            return Ok(userDto);
        }
    }
}