using AutoMapper;

using FlashcardApp.Api.Dtos.CardDtos;
using FlashcardApp.Api.Interfaces;

namespace FlashcardApp.Api.Services
{
    public class CardsService : ICardsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUploadsService _uploadsService;
        private readonly ILogger<CardsService> _logger;

        public CardsService(IUnitOfWork unitOfWork, IMapper mapper, IUploadsService uploadsService, ILogger<CardsService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _uploadsService = uploadsService;
            _logger = logger;
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

            var settings = await _unitOfWork.SettingsRepository.GetSettingsByUserIdAsync(userId);
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

            // Upload the image if provided
            if (createCardDto.ImageFile is not null && createCardDto.ImageFile.Length > 0)
            {
                var uploadResult = await _uploadsService.UploadImageAsync(createCardDto.ImageFile);
                if (!uploadResult.IsSuccess)
                {
                    return ServiceResult<CardResponseDto>.Failure(
                        uploadResult.ErrorMessage,
                        HttpStatusCode.InternalServerError
                    );
                }

                card.ImageUrl = uploadResult.SecureUrl;
                card.ImagePublicId = uploadResult.PublicId;
            }

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

            // Upload the image if provided
            if (updateCardRequestDto.ImageFile is not null && updateCardRequestDto.ImageFile.Length > 0)
            {
                var uploadResult = await _uploadsService.UploadImageAsync(updateCardRequestDto.ImageFile);
                if (!uploadResult.IsSuccess)
                {
                    return ServiceResult<CardResponseDto>.Failure(
                        uploadResult.ErrorMessage,
                        HttpStatusCode.InternalServerError
                    );
                }

                // Delete the old image if it exists
                var oldImagePublicId = card.ImagePublicId;
                if (!string.IsNullOrEmpty(oldImagePublicId))
                {
                    var deleteResult = await _uploadsService.DeleteImageAsync(oldImagePublicId);
                    if (!deleteResult.IsSuccess)
                    {
                        return ServiceResult<CardResponseDto>.Failure(
                            deleteResult.ErrorMessage ?? "Failed to delete old image",
                            HttpStatusCode.InternalServerError
                        );
                    }
                }

                card.ImageUrl = uploadResult.SecureUrl;
                card.ImagePublicId = uploadResult.PublicId;
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

            // Delete the image if it exists
            if (!string.IsNullOrEmpty(card.ImagePublicId))
            {
                var deleteResult = await _uploadsService.DeleteImageAsync(card.ImagePublicId);
                if (!deleteResult.IsSuccess)
                {
                    _logger.LogError("Failed to delete card image: {Error}", deleteResult.ErrorMessage);
                }
            }

            await _unitOfWork.CardsRepository.TryDeleteAsync(card);
            await _unitOfWork.SaveAsync();

            return ServiceResult<object>.Success(
                new { Message = "Card deleted successfully" },
                HttpStatusCode.NoContent);
        }
    }
}
