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
        public async Task<ActionResult<ServiceResult<ProfileResponse>>> GetProfile()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ServiceResult<ProfileResponse>.Failure(
                    "Validation failed",
                    HttpStatusCode.BadRequest
                ));
            }

            var result = await _usersService.GetProfile(User);
            return result.ToActionResult();
        }

        [HttpPost("update-username")]
        public async Task<ActionResult<ServiceResult<ProfileResponse>>> UpdateUsername([FromBody] UpdateUsernameRequest updateUsernameRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ServiceResult<ProfileResponse>.Failure(
                    "Validation failed",
                    HttpStatusCode.BadRequest
                ));
            }
            var result = await _usersService.UpdateUsername(updateUsernameRequest, User);
            return result.ToActionResult();
        }

        [HttpPost("update-avatar")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<ServiceResult<ProfileResponse>>> UpdateAvatar([FromForm] UpdateAvatarRequest updateAvatarRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ServiceResult<ProfileResponse>.Failure(
                    "Validation failed",
                    HttpStatusCode.BadRequest
                ));
            }

            if (updateAvatarRequest.File == null || updateAvatarRequest.File.Length == 0)
            {
                return BadRequest(ServiceResult<ProfileResponse>.Failure(
                    "Avatar file is required",
                    HttpStatusCode.BadRequest
                ));
            }

            var result = await _usersService.UpdateAvatar(updateAvatarRequest, User);
            return result.ToActionResult();
        }
    }
}
