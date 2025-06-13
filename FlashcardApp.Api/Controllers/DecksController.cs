using FlashcardApp.Api.Dtos.DeckDtos;

using Microsoft.AspNetCore.Authorization;

namespace FlashcardApp.Api.Controllers
{
    [Authorize]
    [Route("api/decks")]
    [ApiController]
    public class DecksController : ControllerBase
    {
        private readonly IDecksService _decksService;

        public DecksController(IDecksService decksService)
        {
            _decksService = decksService;
        }

        [HttpGet]
        public async Task<ActionResult<ServiceResult<ICollection<DeckResponseDto>>>> GetDecks([FromQuery] PaginationQuery? paginationQuery)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ServiceResult<ICollection<DeckResponseDto>>.Failure(
                    "Validation failed",
                    HttpStatusCode.BadRequest
                ));
            }

            var result = await _decksService.GetDecksByUserIdAsync(paginationQuery, User);
            return result.ToActionResult();
        }

        [HttpGet("{deckId:int}")]
        public async Task<ActionResult<ServiceResult<DeckResponseDto>>> GetDeckById([FromRoute] int deckId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ServiceResult<DeckResponseDto>.Failure(
                    "Validation failed",
                    HttpStatusCode.BadRequest
                ));
            }

            var result = await _decksService.GetDeckByIdAsync(deckId, User);
            return result.ToActionResult();
        }

        [HttpPost]
        public async Task<ActionResult<ServiceResult<DeckResponseDto>>> CreateDeck([FromBody] CreateDeckRequestDto createDeckRequestDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ServiceResult<DeckResponseDto>.Failure(
                    "Validation failed",
                    HttpStatusCode.BadRequest
                ));
            }

            var result = await _decksService.CreateDeckAsync(createDeckRequestDto, User);
            return result.ToActionResult();
        }

        [HttpPut("{deckId:int}")]
        public async Task<ActionResult<ServiceResult<DeckResponseDto>>> UpdateDeck([FromRoute] int deckId, [FromBody] UpdateDeckRequestDto updateDeckRequestDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ServiceResult<DeckResponseDto>.Failure(
                    "Validation failed",
                    HttpStatusCode.BadRequest
                ));
            }

            var result = await _decksService.UpdateDeckAsync(deckId, updateDeckRequestDto, User);
            return result.ToActionResult();
        }

        [HttpDelete("{deckId:int}")]
        public async Task<ActionResult<ServiceResult<object>>> DeleteDeck([FromRoute] int deckId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ServiceResult<object>.Failure(
                    "Validation failed",
                    HttpStatusCode.BadRequest
                ));
            }

            var result = await _decksService.DeleteDeckAsync(deckId, User);
            return result.ToActionResult();
        }

        // <summary>
        // PATCH /api/decks/{deckId}/sharing
        // </summary>
    }
}
