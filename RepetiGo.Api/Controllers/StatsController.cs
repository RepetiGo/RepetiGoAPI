using Microsoft.AspNetCore.Authorization;

using RepetiGo.Api.Dtos.StatsDtos;

namespace RepetiGo.Api.Controllers
{
    [Authorize]
    [Route("api/stats")]
    [ApiController]
    public class StatsController : ControllerBase
    {
        private readonly IReviewsService _reviewsService;

        public StatsController(IReviewsService reviewsService)
        {
            _reviewsService = reviewsService;
        }

        [HttpGet]
        public async Task<ActionResult<ServiceResult<ActivityStatsResponse>>> GetReviews([FromQuery] int year)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ServiceResult<ActivityStatsResponse>.Failure(
                        "Validation failed",
                        HttpStatusCode.BadRequest
                    ));
            }

            var result = await _reviewsService.GetReviewsAsync(year, User);
            return result.ToActionResult();
        }
    }
}
