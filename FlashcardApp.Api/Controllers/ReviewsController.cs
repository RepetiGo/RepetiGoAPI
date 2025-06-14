using FlashcardApp.Api.Dtos.CardDtos;
using FlashcardApp.Api.Dtos.ReviewDtos;

using Microsoft.AspNetCore.Authorization;

namespace FlashcardApp.Api.Controllers
{
    [Authorize]
    [Route("api/reviews")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewsService _reviewsService;

        public ReviewsController(IReviewsService reviewsService)
        {
            _reviewsService = reviewsService;
        }

        [HttpGet("decks/{deckId:int}/cards")]
        public async Task<ActionResult<ServiceResult<ICollection<CardResponseDto>>>> GetDueCardsByDeckIdAsync([FromRoute] int deckId, [FromQuery] PaginationQuery? paginationQuery)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ServiceResult<ICollection<CardResponseDto>>.Failure(
                    "Validation failed",
                    HttpStatusCode.BadRequest
                ));
            }

            var result = await _reviewsService.GetDueCardsByDeckIdAsync(deckId, paginationQuery, User);
            return result.ToActionResult();
        }

        [HttpPost("decks/{deckId:int}/cards/{cardId:int}")]
        public async Task<ActionResult<ServiceResult<CardResponseDto>>> ReviewCardAsync([FromRoute] int deckId, [FromRoute] int cardId, [FromBody] ReviewRequestDto reviewRequestDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ServiceResult<CardResponseDto>.Failure(
                    "Validation failed",
                    HttpStatusCode.BadRequest
                ));
            }

            var result = await _reviewsService.ReviewCardAsync(deckId, cardId, reviewRequestDto, User);
            return result.ToActionResult();
        }
    }
}
