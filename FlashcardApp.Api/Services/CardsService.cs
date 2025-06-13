

using AutoMapper;

using FlashcardApp.Api.Dtos.CardDtos;

namespace FlashcardApp.Api.Services
{
    public class CardsService : ICardsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ISettingsService _settingsService;

        public CardsService(IUnitOfWork unitOfWork, IMapper mapper, ISettingsService settingsService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _settingsService = settingsService;
        }

        public async Task<ServiceResult<ICollection<CardResponseDto>>> GetCardsByDeckIdAsync(int deckId, PaginationQuery? paginationQuery, ClaimsPrincipal claimsPrincipal)
        {
            var userId = claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return ServiceResult<ICollection<CardResponseDto>>.Failure(
                    "User not authenticated",
                    HttpStatusCode.Unauthorized
                );
            }

            var deck = await _unitOfWork.DecksRepository.GetByIdAsync(deckId);
            if (deck is null)
            {
                return ServiceResult<ICollection<CardResponseDto>>.Failure(
                    "Deck not found",
                    HttpStatusCode.NotFound
                );
            }

            if (deck.UserId != userId)
            {
                return ServiceResult<ICollection<CardResponseDto>>.Failure(
                    "You do not have permission to access this deck",
                    HttpStatusCode.Forbidden
                );
            }

            var cards = await _unitOfWork.CardsRepository.GetAllAsync(
                filter: c => c.DeckId == deckId,
                orderBy: q => q.OrderBy(c => c.CreatedAt),
                paginationQuery: paginationQuery);

            var cardDto = _mapper.Map<ICollection<CardResponseDto>>(cards);

            return ServiceResult<ICollection<CardResponseDto>>.Success(cardDto, HttpStatusCode.OK);
        }

        public async Task<ServiceResult<CardResponseDto>> GetCardByIdAsync(int deckId, int cardId, ClaimsPrincipal claimsPrincipal)
        {
            var userId = claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return ServiceResult<CardResponseDto>.Failure(
                    "User not authenticated",
                    HttpStatusCode.Unauthorized
                );
            }

            var deck = await _unitOfWork.DecksRepository.GetByIdAsync(deckId);
            if (deck is null)
            {
                return ServiceResult<CardResponseDto>.Failure(
                    "Deck not found",
                    HttpStatusCode.NotFound
                );
            }

            if (deck.UserId != userId)
            {
                return ServiceResult<CardResponseDto>.Failure(
                    "You do not have permission to access this deck",
                    HttpStatusCode.Forbidden
                );
            }

            var card = await _unitOfWork.CardsRepository.GetByIdAsync(cardId);
            if (card is null || card.DeckId != deckId)
            {
                return ServiceResult<CardResponseDto>.Failure(
                    "Card not found",
                    HttpStatusCode.NotFound
                );
            }

            var cardDto = _mapper.Map<CardResponseDto>(card);

            return ServiceResult<CardResponseDto>.Success(cardDto, HttpStatusCode.OK);
        }

        public async Task<ServiceResult<CardResponseDto>> CreateCardAsync(int deckId, CreateCardRequestDto createCardDto, ClaimsPrincipal claimsPrincipal)
        {
            var userId = claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return ServiceResult<CardResponseDto>.Failure(
                    "User not authenticated",
                    HttpStatusCode.Unauthorized
                );
            }

            var deck = await _unitOfWork.DecksRepository.GetByIdAsync(deckId);
            if (deck is null)
            {
                return ServiceResult<CardResponseDto>.Failure(
                    "Deck not found",
                    HttpStatusCode.NotFound
                );
            }

            if (deck.UserId != userId)
            {
                return ServiceResult<CardResponseDto>.Failure(
                    "You do not have permission to access this deck",
                    HttpStatusCode.Forbidden
                );
            }

            var settings = await _settingsService.GetSettingsByUserIdAsync(userId);
            if (settings is null)
            {
                return ServiceResult<CardResponseDto>.Failure(
                    "User settings not found",
                    HttpStatusCode.NotFound
                );
            }

            var card = _mapper.Map<Card>(createCardDto);

            card.DeckId = deckId;
            card.EasinessFactor = settings.StartingEasinessFactor;
            await _unitOfWork.CardsRepository.AddAsync(card);
            await _unitOfWork.SaveAsync();

            var cardDto = _mapper.Map<CardResponseDto>(card);

            return ServiceResult<CardResponseDto>.Success(cardDto, HttpStatusCode.Created);
        }

        public async Task<ServiceResult<CardResponseDto>> UpdateCardAsync(int deckId, int cardId, UpdateCardRequestDto updateCardRequestDto, ClaimsPrincipal claimsPrincipal)
        {
            var userId = claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return ServiceResult<CardResponseDto>.Failure(
                    "User not authenticated",
                    HttpStatusCode.Unauthorized
                );
            }

            var deck = await _unitOfWork.DecksRepository.GetByIdAsync(deckId);
            if (deck is null)
            {
                return ServiceResult<CardResponseDto>.Failure(
                    "Deck not found",
                    HttpStatusCode.NotFound
                );
            }

            if (deck.UserId != userId)
            {
                return ServiceResult<CardResponseDto>.Failure(
                    "You do not have permission to access this deck",
                    HttpStatusCode.Forbidden
                );
            }

            var card = await _unitOfWork.CardsRepository.GetByIdAsync(cardId);
            if (card is null || card.DeckId != deckId)
            {
                return ServiceResult<CardResponseDto>.Failure(
                    "Card not found",
                    HttpStatusCode.NotFound
                );
            }

            _mapper.Map(updateCardRequestDto, card);
            await _unitOfWork.CardsRepository.UpdateAsync(card);
            await _unitOfWork.SaveAsync();

            var cardDto = _mapper.Map<CardResponseDto>(card);

            return ServiceResult<CardResponseDto>.Success(cardDto, HttpStatusCode.OK);
        }

        public async Task<ServiceResult<object>> DeleteCardAsync(int deckId, int cardId, ClaimsPrincipal claimsPrincipal)
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
                    "You do not have permission to access this deck",
                    HttpStatusCode.Forbidden
                );
            }

            var card = await _unitOfWork.CardsRepository.GetByIdAsync(cardId);
            if (card is null || card.DeckId != deckId)
            {
                return ServiceResult<object>.Failure(
                    "Card not found",
                    HttpStatusCode.NotFound
                );
            }

            await _unitOfWork.CardsRepository.TryDeleteAsync(card);
            await _unitOfWork.SaveAsync();

            return ServiceResult<object>.Success(
                new { Message = "Card deleted successfully" },
                HttpStatusCode.NoContent);
        }
    }
}
