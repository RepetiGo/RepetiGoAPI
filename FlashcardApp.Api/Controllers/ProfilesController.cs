using FlashcardApp.Api.Dtos.ProfileDtos;

using Microsoft.AspNetCore.Authorization;

namespace FlashcardApp.Api.Controllers
{
    [Authorize]
    [Route("api/profiles")]
    [ApiController]
    public class ProfilesController : ControllerBase
    {
        private readonly IUsersService _usersService;

        public ProfilesController(IUsersService usersService)
        {
            _usersService = usersService;
        }

        [HttpGet("profile")]
        public async Task<ActionResult<ServiceResult<ProfileResponseDto>>> GetProfile()
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

        [HttpPost("update-username")]
        public async Task<ActionResult<ServiceResult<ProfileResponseDto>>> UpdateUsername([FromBody] UpdateUsernameRequestDto updateUsernameRequestDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ServiceResult<ProfileResponseDto>.Failure(
                    "Validation failed",
                    HttpStatusCode.BadRequest
                ));
            }
            var result = await _usersService.UpdateUsername(updateUsernameRequestDto, User);
            return result.ToActionResult();
        }

        [HttpPost("update-avatar")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<ServiceResult<ProfileResponseDto>>> UpdateAvatar([FromForm] UpdateAvatarRequestDto updateAvatarRequestDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ServiceResult<ProfileResponseDto>.Failure(
                    "Validation failed",
                    HttpStatusCode.BadRequest
                ));
            }

            if (updateAvatarRequestDto.File == null || updateAvatarRequestDto.File.Length == 0)
            {
                return BadRequest(ServiceResult<ProfileResponseDto>.Failure(
                    "Avatar file is required",
                    HttpStatusCode.BadRequest
                ));
            }

            var result = await _usersService.UpdateAvatar(updateAvatarRequestDto, User);
            return result.ToActionResult();
        }
    }
}
