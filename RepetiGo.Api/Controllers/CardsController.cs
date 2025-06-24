using Microsoft.AspNetCore.Authorization;

using RepetiGo.Api.Dtos.CardDtos;
using RepetiGo.Api.Dtos.GeneratedCardDtos;
using RepetiGo.Api.Dtos.ReviewDtos;

namespace RepetiGo.Api.Controllers
{
    [Authorize]
    [Route("api/cards")]
    [ApiController]
    public class CardsController : ControllerBase
    {
        private readonly ICardsService _cardsService;

        public CardsController(ICardsService cardsService)
        {
            _cardsService = cardsService;
        }

        [HttpGet("decks/{deckId:int}/cards")]
        public async Task<ActionResult<ServiceResult<ICollection<CardResponse>>>> GetCardsByDeckIdAsync([FromRoute] int deckId, [FromQuery] Query? query)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ServiceResult<ICollection<CardResponse>>.Failure(
                    "Validation failed",
                    HttpStatusCode.BadRequest
                ));
            }

            var result = await _cardsService.GetCardsByDeckIdAsync(deckId, query, User);
            return result.ToActionResult();
        }

        [HttpGet("decks/{deckId:int}/cards/{cardId:int}")]
        public async Task<ActionResult<ServiceResult<CardResponse>>> GetCardByIdAsync([FromRoute] int deckId, [FromRoute] int cardId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ServiceResult<CardResponse>.Failure(
                    "Validation failed",
                    HttpStatusCode.BadRequest
                ));
            }

            var result = await _cardsService.GetCardByIdAsync(deckId, cardId, User);
            return result.ToActionResult();
        }

        [HttpPost("decks/{deckId:int}/cards")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<ServiceResult<CardResponse>>> CreateCardAsync([FromRoute] int deckId, [FromForm] CreateCardRequest createCardRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ServiceResult<CardResponse>.Failure(
                    "Validation failed",
                    HttpStatusCode.BadRequest
                ));
            }

            var result = await _cardsService.CreateCardAsync(deckId, createCardRequest, User);
            return result.ToActionResult();
        }

        [HttpPut("decks/{deckId:int}/cards/{cardId:int}")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<ServiceResult<CardResponse>>> UpdateCardAsync([FromRoute] int deckId, [FromRoute] int cardId, [FromForm] UpdateCardRequest updateCardRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ServiceResult<CardResponse>.Failure(
                    "Validation failed",
                    HttpStatusCode.BadRequest
                ));
            }

            var result = await _cardsService.UpdateCardAsync(deckId, cardId, updateCardRequest, User);
            return result.ToActionResult();
        }

        [HttpDelete("decks/{deckId:int}/cards/{cardId:int}")]
        public async Task<ActionResult<ServiceResult<object>>> DeleteCardAsync([FromRoute] int deckId, [FromRoute] int cardId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ServiceResult<object>.Failure(
                    "Validation failed",
                    HttpStatusCode.BadRequest
                ));
            }

            var result = await _cardsService.DeleteCardAsync(deckId, cardId, User);
            return result.ToActionResult();
        }

        [HttpPost("generate")]
        //[EnableRateLimiting("ai-generation")]
        public async Task<ActionResult<ServiceResult<PreviewCardResponse>>> GenerateCardAsync([FromBody] GenerateRequest generateRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ServiceResult<PreviewCardResponse>.Failure(
                    "Validation failed",
                    HttpStatusCode.BadRequest
                ));
            }

            var result = await _cardsService.GenerateCardAsync(generateRequest, User);
            return result.ToActionResult();
        }

        [HttpGet("decks/{deckId:int}/due")]
        public async Task<ActionResult<ServiceResult<ICollection<ReviewResponse>>>> GetDueCardsByDeckIdAsync([FromRoute] int deckId, [FromQuery] Query? query)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ServiceResult<ICollection<ReviewResponse>>.Failure(
                    "Validation failed",
                    HttpStatusCode.BadRequest
                ));
            }

            var result = await _cardsService.GetDueCardsByDeckIdAsync(deckId, query, User);
            return result.ToActionResult();
        }

        [HttpPut("decks/{deckId:int}/cards/{cardId:int}/review")]
        public async Task<ActionResult<ServiceResult<object>>> ReviewCardAsync([FromRoute] int deckId, [FromRoute] int cardId, [FromBody] ReviewRequest reviewRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ServiceResult<object>.Failure(
                    "Validation failed",
                    HttpStatusCode.BadRequest
                ));
            }

            var result = await _cardsService.ReviewCardAsync(deckId, cardId, reviewRequest, User);
            return result.ToActionResult();
        }
    }
}
