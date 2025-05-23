using backend.Dtos.AuthDtos;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ITokenService _tokenService;

        public AuthController(UserManager<IdentityUser> userManager, ITokenService tokenService)
        {
            _userManager = userManager;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<ResponseDto>> Register([FromBody] RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new IdentityUser
            {
                UserName = registerDto.Email,
                Email = registerDto.Email,
                EmailConfirmed = true // Set to true if you want to confirm the email immediately
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded)
            {
                return BadRequest(
                    new ResponseErrorDto
                    (
                        HttpStatusCode.BadRequest,
                        result.Errors.FirstOrDefault()?.Description ?? "User registration failed",
                        "Bad Request"
                    )
                );
            }

            return Ok(
                new ResponseDto
                {
                    Email = user.Email,
                    Token = _tokenService.GenerateAccessToken(user)
                }
            );
        }

        [HttpPost("login")]
        public async Task<ActionResult<ResponseDto>> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByEmailAsync(loginDto.Email);

            if (user is null)
            {
                return Unauthorized(new ResponseErrorDto
                (
                    HttpStatusCode.Unauthorized,
                    "User not found",
                    "Unauthorized"
                ));
            }

            var isValid = await _userManager.CheckPasswordAsync(user, loginDto.Password);

            if (!isValid)
            {
                return Unauthorized(new ResponseErrorDto
                (
                     HttpStatusCode.Unauthorized,
                     "Password is incorrect",
                     "Unauthorized"
                ));
            }

            return Ok(
                new ResponseDto
                {
                    Email = user.Email!,
                    Token = _tokenService.GenerateAccessToken(user)
                }
            );
        }
    }
}
