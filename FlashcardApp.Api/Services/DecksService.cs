using AutoMapper;

using FlashcardApp.Api.Dtos.DeckDtos;
using FlashcardApp.Api.Interfaces;

namespace FlashcardApp.Api.Services
{
    public class DecksService : IDecksService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUploadsService _uploadsService;
        private readonly ILogger<DecksService> _logger;

        public DecksService(UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork, IMapper mapper, IUploadsService uploadsService, ILogger<DecksService> logger)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _uploadsService = uploadsService;
            _logger = logger;
        }

        public async Task<ServiceResult<ICollection<DeckResponseDto>>> GetDecksByUserIdAsync(PaginationQuery? paginationQuery, ClaimsPrincipal claimsPrincipal)
        {
            var userId = claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return ServiceResult<ICollection<DeckResponseDto>>.Failure(
                    "User not authenticated",
                    HttpStatusCode.Unauthorized
                );
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
            {
                return ServiceResult<ICollection<DeckResponseDto>>.Failure(
                    "User not found",
                    HttpStatusCode.NotFound
                );
            }

            var decks = await _unitOfWork.DecksRepository.GetAllAsync(
                filter: d => d.UserId == userId,
                orderBy: q => q.OrderBy(d => d.CreatedAt),
                paginationQuery: paginationQuery);

            var deckDtos = _mapper.Map<ICollection<DeckResponseDto>>(decks);

            return ServiceResult<ICollection<DeckResponseDto>>.Success(deckDtos);
        }

        public async Task<ServiceResult<DeckResponseDto>> GetDeckByIdAsync(int deckId, ClaimsPrincipal claimsPrincipal)
        {
            var userId = claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return ServiceResult<DeckResponseDto>.Failure(
                    "User not authenticated",
                    HttpStatusCode.Unauthorized
                );
            }

            var deck = await _unitOfWork.DecksRepository.GetByIdAsync(deckId);
            if (deck is null)
            {
                return ServiceResult<DeckResponseDto>.Failure(
                    "Deck not found",
                    HttpStatusCode.NotFound
                );
            }

            if (deck.UserId != userId)
            {
                return ServiceResult<DeckResponseDto>.Failure(
                    "You do not have permission to access this deck",
                    HttpStatusCode.Forbidden
                );
            }

            var deckDto = _mapper.Map<DeckResponseDto>(deck);

            return ServiceResult<DeckResponseDto>.Success(deckDto);
        }

        public async Task<ServiceResult<DeckResponseDto>> CreateDeckAsync(CreateDeckRequestDto createDeckDto, ClaimsPrincipal claimsPrincipal)
        {
            var userId = claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return ServiceResult<DeckResponseDto>.Failure(
                    "User not authenticated",
                    HttpStatusCode.Unauthorized
                );
            }

            var deck = new Deck
            {
                Name = createDeckDto.Name,
                Description = createDeckDto.Description,
                Visibility = createDeckDto.Visibility,
                UserId = userId,
            };

            await _unitOfWork.DecksRepository.AddAsync(deck);
            await _unitOfWork.SaveAsync();

            var existingDeck = await _unitOfWork.DecksRepository.GetByIdAsync(deck.Id);
            if (existingDeck is null)
            {
                return ServiceResult<DeckResponseDto>.Failure(
                    "Failed to create deck",
                    HttpStatusCode.InternalServerError
                );
            }

            var deckDto = _mapper.Map<DeckResponseDto>(existingDeck);

            return ServiceResult<DeckResponseDto>.Success(deckDto);
        }

        public async Task<ServiceResult<DeckResponseDto>> UpdateDeckAsync(int deckId, UpdateDeckRequestDto updateDeckRequestDto, ClaimsPrincipal claimsPrincipal)
        {
            var userId = claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return ServiceResult<DeckResponseDto>.Failure(
                    "User not authenticated",
                    HttpStatusCode.Unauthorized
                );
            }

            var deck = await _unitOfWork.DecksRepository.GetByIdAsync(deckId);
            if (deck is null)
            {
                return ServiceResult<DeckResponseDto>.Failure(
                    "Deck not found",
                    HttpStatusCode.NotFound
                );
            }

            if (deck.UserId != userId)
            {
                return ServiceResult<DeckResponseDto>.Failure(
                    "You do not have permission to update this deck",
                    HttpStatusCode.Forbidden
                );
            }

            _mapper.Map(updateDeckRequestDto, deck);
            await _unitOfWork.DecksRepository.UpdateAsync(deck);
            await _unitOfWork.SaveAsync();

            var updatedDeck = await _unitOfWork.DecksRepository.GetByIdAsync(deckId);
            if (updatedDeck is null)
            {
                return ServiceResult<DeckResponseDto>.Failure(
                    "Failed to retrieve updated deck",
                    HttpStatusCode.InternalServerError
                );
            }

            var deckDto = _mapper.Map<DeckResponseDto>(updatedDeck);

            return ServiceResult<DeckResponseDto>.Success(deckDto);
        }

        public async Task<ServiceResult<object>> DeleteDeckAsync(int deckId, ClaimsPrincipal claimsPrincipal)
        {
            var userId = claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return ServiceResult<object>.Failure(
                    "User not authenticated",
                    HttpStatusCode.Unauthorized
                );
            }

            var deck = await _unitOfWork.DecksRepository.GetByIdAsync(deckId);
            if (deck is null)
            {
                return ServiceResult<object>.Failure(
                    "Deck not found",
                    HttpStatusCode.NotFound
                );
            }

            if (deck.UserId != userId)
            {
                return ServiceResult<object>.Failure(
                    "You do not have permission to delete this deck",
                    HttpStatusCode.Forbidden
                );
            }

            // Check if the deck has any associated flashcards
            var cards = await _unitOfWork.CardsRepository.GetAllAsync(c => c.DeckId == deckId);
            if (cards != null && cards.Any())
            {
                foreach (var card in cards)
                {
                    var imageDeletionResult = await _uploadsService.DeleteImageAsync(card.ImagePublicId);
                    if (!imageDeletionResult.IsSuccess)
                    {
                        _logger.LogError("Failed to delete card image: {Error}", imageDeletionResult.ErrorMessage);
                    }
                }
            }

            await _unitOfWork.DecksRepository.TryDeleteAsync(deck);
            await _unitOfWork.SaveAsync();

            return ServiceResult<object>.Success(
                new { Message = "Deck deleted successfully" },
                HttpStatusCode.NoContent
            );
        }
    }
}
