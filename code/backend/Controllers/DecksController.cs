namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DecksController : ControllerBase
    {
        private readonly IDecksService _decksService;

        public DecksController(IDecksService decksService)
        {
            _decksService = decksService;
        }

        [HttpGet]
        public async Task<ActionResult<ICollection<Deck>>> GetDecks()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ServiceResult<ICollection<Deck>>.Failure(
                    "Validation failed",
                    HttpStatusCode.BadRequest
                ));
            }

            var result = await _decksService.GetDecksByUserIdAsync(User);
            return result.ToActionResult();
        }

        [HttpGet("{deckId:int}")]
        public async Task<ActionResult<Deck>> GetDeckById(int deckId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ServiceResult<Deck>.Failure(
                    "Validation failed",
                    HttpStatusCode.BadRequest
                ));
            }

            var result = await _decksService.GetDeckByIdAsync(deckId, User);
            return result.ToActionResult();
        }

        [HttpPost]
        public async Task<ActionResult<Deck>> CreateDeck([FromBody] CreateDeckRequestDto createDeckDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ServiceResult<Deck>.Failure(
                    "Validation failed",
                    HttpStatusCode.BadRequest
                ));
            }

            var result = await _decksService.CreateDeckAsync(createDeckDto, User);
            return result.ToActionResult();
        }

        [HttpPut("{deckId:int}")]
        public async Task<ActionResult<Deck>> UpdateDeck(int deckId, [FromBody] UpdateDeckRequestDto updateDeckDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ServiceResult<Deck>.Failure(
                    "Validation failed",
                    HttpStatusCode.BadRequest
                ));
            }

            updateDeckDto.Id = deckId; // Ensure the ID is set for the update
            var result = await _decksService.UpdateDeckAsync(updateDeckDto, User);
            return result.ToActionResult();
        }
    }
}
