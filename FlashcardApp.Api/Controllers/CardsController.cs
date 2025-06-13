using FlashcardApp.Api.Dtos.CardDtos;

using Microsoft.AspNetCore.Authorization;

namespace FlashcardApp.Api.Controllers
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
        public async Task<ActionResult<ServiceResult<ICollection<CardResponseDto>>>> GetCardsByDeckIdAsync([FromRoute] int deckId, [FromQuery] PaginationQuery? paginationQuery)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ServiceResult<ICollection<CardResponseDto>>.Failure(
                    "Validation failed",
                    HttpStatusCode.BadRequest
                ));
            }

            var result = await _cardsService.GetCardsByDeckIdAsync(deckId, paginationQuery, User);
            return result.ToActionResult();
        }

        [HttpGet("decks/{deckId:int}/cards/{cardId:int}")]
        public async Task<ActionResult<ServiceResult<CardResponseDto>>> GetCardByIdAsync([FromRoute] int deckId, [FromRoute] int cardId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ServiceResult<CardResponseDto>.Failure(
                    "Validation failed",
                    HttpStatusCode.BadRequest
                ));
            }

            var result = await _cardsService.GetCardByIdAsync(deckId, cardId, User);
            return result.ToActionResult();
        }

        [HttpPost("decks/{deckId:int}/cards")]
        public async Task<ActionResult<ServiceResult<CardResponseDto>>> CreateCardAsync([FromRoute] int deckId, [FromBody] CreateCardRequestDto createCardDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ServiceResult<CardResponseDto>.Failure(
                    "Validation failed",
                    HttpStatusCode.BadRequest
                ));
            }

            var result = await _cardsService.CreateCardAsync(deckId, createCardDto, User);
            return result.ToActionResult();
        }

        [HttpPut("decks/{deckId:int}/cards/{cardId:int}")]
        public async Task<ActionResult<ServiceResult<CardResponseDto>>> UpdateCardAsync([FromRoute] int deckId, [FromRoute] int cardId, [FromBody] UpdateCardRequestDto updateCardRequestDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ServiceResult<CardResponseDto>.Failure(
                    "Validation failed",
                    HttpStatusCode.BadRequest
                ));
            }

            var result = await _cardsService.UpdateCardAsync(deckId, cardId, updateCardRequestDto, User);
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
    }
}
