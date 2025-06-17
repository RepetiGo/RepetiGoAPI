using Microsoft.AspNetCore.Authorization;

using RepetiGo.Api.Dtos.DeckDtos;

namespace RepetiGo.Api.Controllers
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
        public async Task<ActionResult<ServiceResult<ICollection<DeckResponse>>>> GetDecks([FromQuery] PaginationQuery? paginationQuery)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ServiceResult<ICollection<DeckResponse>>.Failure(
                    "Validation failed",
                    HttpStatusCode.BadRequest
                ));
            }

            var result = await _decksService.GetDecksByUserIdAsync(paginationQuery, User);
            return result.ToActionResult();
        }

        [HttpGet("{deckId:int}")]
        public async Task<ActionResult<ServiceResult<DeckResponse>>> GetDeckById([FromRoute] int deckId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ServiceResult<DeckResponse>.Failure(
                    "Validation failed",
                    HttpStatusCode.BadRequest
                ));
            }

            var result = await _decksService.GetDeckByIdAsync(deckId, User);
            return result.ToActionResult();
        }

        [HttpPost]
        public async Task<ActionResult<ServiceResult<DeckResponse>>> CreateDeck([FromBody] CreateDeckRequest createDeckRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ServiceResult<DeckResponse>.Failure(
                    "Validation failed",
                    HttpStatusCode.BadRequest
                ));
            }

            var result = await _decksService.CreateDeckAsync(createDeckRequest, User);
            return result.ToActionResult();
        }

        [HttpPut("{deckId:int}")]
        public async Task<ActionResult<ServiceResult<DeckResponse>>> UpdateDeck([FromRoute] int deckId, [FromBody] UpdateDeckRequest updateDeckRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ServiceResult<DeckResponse>.Failure(
                    "Validation failed",
                    HttpStatusCode.BadRequest
                ));
            }

            var result = await _decksService.UpdateDeckAsync(deckId, updateDeckRequest, User);
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
        // GET /api/shared/decks
        // POST /api/shared/decks/{deckId}/import
        // </summary>
    }
}
