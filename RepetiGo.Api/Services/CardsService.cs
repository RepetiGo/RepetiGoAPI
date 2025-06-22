using AutoMapper;

using RepetiGo.Api.Dtos.CardDtos;
using RepetiGo.Api.Dtos.GeneratedCardDtos;
using RepetiGo.Api.Dtos.ReviewDtos;

namespace RepetiGo.Api.Services
{
    public class CardsService : ICardsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUploadsService _uploadsService;
        private readonly ILogger<CardsService> _logger;
        private readonly IAiGeneratorService _aiGeneratorService;
        private readonly IReviewsService _reviewsService;
        private readonly IDecksService _decksService;

        public CardsService(IUnitOfWork unitOfWork, IMapper mapper, IUploadsService uploadsService, ILogger<CardsService> logger, IAiGeneratorService aiGeneratorService, IReviewsService reviewsService, IDecksService decksService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _uploadsService = uploadsService;
            _logger = logger;
            _aiGeneratorService = aiGeneratorService;
            _reviewsService = reviewsService;
            _decksService = decksService;
        }

        public async Task<ServiceResult<ICollection<CardResponse>>> GetCardsByDeckIdAsync(int deckId, Query? query, ClaimsPrincipal claimsPrincipal)
        {
            var userId = claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return ServiceResult<ICollection<CardResponse>>.Failure(
                    "User not authenticated",
                    HttpStatusCode.Unauthorized
                );
            }

            var deck = await _unitOfWork.DecksRepository.GetByIdAsync(deckId);
            if (deck is null)
            {
                return ServiceResult<ICollection<CardResponse>>.Failure(
                    "Deck not found",
                    HttpStatusCode.NotFound
                );
            }

            if (!_decksService.HasAccessToDeck(deck, userId))
            {
                return ServiceResult<ICollection<CardResponse>>.Failure(
                    "You do not have permission to access this deck",
                    HttpStatusCode.Forbidden
                );
            }

            var cards = await _unitOfWork.CardsRepository.GetAllAsync(
                filter: c => c.DeckId == deckId,
                query: query);

            var cardResponse = _mapper.Map<ICollection<CardResponse>>(cards);

            return ServiceResult<ICollection<CardResponse>>.Success(cardResponse, HttpStatusCode.OK);
        }

        public async Task<ServiceResult<CardResponse>> GetCardByIdAsync(int deckId, int cardId, ClaimsPrincipal claimsPrincipal)
        {
            var userId = claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return ServiceResult<CardResponse>.Failure(
                    "User not authenticated",
                    HttpStatusCode.Unauthorized
                );
            }

            var deck = await _unitOfWork.DecksRepository.GetByIdAsync(deckId);
            if (deck is null)
            {
                return ServiceResult<CardResponse>.Failure(
                    "Deck not found",
                    HttpStatusCode.NotFound
                );
            }

            if (!_decksService.HasAccessToDeck(deck, userId))
            {
                return ServiceResult<CardResponse>.Failure(
                    "You do not have permission to access this deck",
                    HttpStatusCode.Forbidden
                );
            }

            var card = await _unitOfWork.CardsRepository.GetByIdAsync(cardId);
            if (card is null || card.DeckId != deckId)
            {
                return ServiceResult<CardResponse>.Failure(
                    $"Card with id {cardId} not found in deck with id {deckId}",
                    HttpStatusCode.NotFound
                );
            }

            var cardResponse = _mapper.Map<CardResponse>(card);

            return ServiceResult<CardResponse>.Success(cardResponse, HttpStatusCode.OK);
        }

        public async Task<ServiceResult<CardResponse>> CreateCardAsync(int deckId, CreateCardRequest createCardRequest, ClaimsPrincipal claimsPrincipal)
        {
            var userId = claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return ServiceResult<CardResponse>.Failure(
                    "User not authenticated",
                    HttpStatusCode.Unauthorized
                );
            }

            var deck = await _unitOfWork.DecksRepository.GetByIdAsync(deckId);
            if (deck is null)
            {
                return ServiceResult<CardResponse>.Failure(
                    "Deck not found",
                    HttpStatusCode.NotFound
                );
            }

            if (deck.UserId != userId)
            {
                return ServiceResult<CardResponse>.Failure(
                    "You do not have permission to access this deck",
                    HttpStatusCode.Forbidden
                );
            }

            var settings = await _unitOfWork.SettingsRepository.GetSettingsByUserIdAsync(userId);
            if (settings is null)
            {
                return ServiceResult<CardResponse>.Failure(
                    "User settings not found",
                    HttpStatusCode.NotFound
                );
            }

            var card = _mapper.Map<Card>(createCardRequest);

            card.DeckId = deckId;
            card.EasinessFactor = settings.StartingEasinessFactor;

            // Check if image url or file is provided
            if (!string.IsNullOrEmpty(createCardRequest.ImageUrl) && !string.IsNullOrEmpty(createCardRequest.ImagePublicId))
            {
                card.ImageUrl = createCardRequest.ImageUrl;
                card.ImagePublicId = createCardRequest.ImagePublicId;
            }
            else if (createCardRequest.ImageFile is not null && createCardRequest.ImageFile.Length > 0)
            {
                var uploadResult = await _uploadsService.UploadImageAsync(createCardRequest.ImageFile);
                if (!uploadResult.IsSuccess)
                {
                    return ServiceResult<CardResponse>.Failure(
                        uploadResult.ErrorMessage ?? string.Empty,
                        HttpStatusCode.InternalServerError
                    );
                }

                card.ImageUrl = uploadResult.SecureUrl;
                card.ImagePublicId = uploadResult.PublicId;
            }

            await _unitOfWork.CardsRepository.AddAsync(card);
            await _unitOfWork.SaveAsync();

            var cardResponse = _mapper.Map<CardResponse>(card);

            return ServiceResult<CardResponse>.Success(cardResponse, HttpStatusCode.Created);
        }

        public async Task<ServiceResult<CardResponse>> UpdateCardAsync(int deckId, int cardId, UpdateCardRequest updateCardRequest, ClaimsPrincipal claimsPrincipal)
        {
            var userId = claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return ServiceResult<CardResponse>.Failure(
                    "User not authenticated",
                    HttpStatusCode.Unauthorized
                );
            }

            var deck = await _unitOfWork.DecksRepository.GetByIdAsync(deckId);
            if (deck is null)
            {
                return ServiceResult<CardResponse>.Failure(
                    "Deck not found",
                    HttpStatusCode.NotFound
                );
            }

            if (deck.UserId != userId)
            {
                return ServiceResult<CardResponse>.Failure(
                    "You do not have permission to access this deck",
                    HttpStatusCode.Forbidden
                );
            }

            var card = await _unitOfWork.CardsRepository.GetByIdAsync(cardId);
            if (card is null || card.DeckId != deckId)
            {
                return ServiceResult<CardResponse>.Failure(
                    "Card not found",
                    HttpStatusCode.NotFound
                );
            }

            // Upload the image if provided
            if (updateCardRequest.ImageFile is not null && updateCardRequest.ImageFile.Length > 0)
            {
                var uploadResult = await _uploadsService.UploadImageAsync(updateCardRequest.ImageFile);
                if (!uploadResult.IsSuccess)
                {
                    return ServiceResult<CardResponse>.Failure(
                        uploadResult.ErrorMessage ?? string.Empty,
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
                        return ServiceResult<CardResponse>.Failure(
                            deleteResult.ErrorMessage ?? "Failed to delete old image",
                            HttpStatusCode.InternalServerError
                        );
                    }
                }

                card.ImageUrl = uploadResult.SecureUrl;
                card.ImagePublicId = uploadResult.PublicId;
            }

            _mapper.Map(updateCardRequest, card);
            await _unitOfWork.CardsRepository.UpdateAsync(card);
            await _unitOfWork.SaveAsync();

            var cardResponse = _mapper.Map<CardResponse>(card);

            return ServiceResult<CardResponse>.Success(cardResponse, HttpStatusCode.OK);
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

        public async Task<ServiceResult<PreviewCardResponse>> GenerateCardAsync(GenerateRequest generateRequest, ClaimsPrincipal claimsPrincipal)
        {
            var userId = claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return ServiceResult<PreviewCardResponse>.Failure(
                    "User not authenticated",
                    HttpStatusCode.Unauthorized
                );
            }

            if (string.IsNullOrWhiteSpace(generateRequest.Topic))
            {
                return ServiceResult<PreviewCardResponse>.Failure(
                    "Topic and deck name cannot be empty",
                    HttpStatusCode.BadRequest
                );
            }

            var generatedContentResult = await _aiGeneratorService.GenerateCardContentAsync(generateRequest);
            if (!generatedContentResult.IsSuccess)
            {
                return ServiceResult<PreviewCardResponse>.Failure(
                    generatedContentResult.ErrorMessage ?? "Failed to generate content",
                    HttpStatusCode.InternalServerError
                );
            }

            generateRequest.FrontText = generatedContentResult.FrontText;
            generateRequest.BackText = generatedContentResult.BackText;

            var generatedImageResult = await _aiGeneratorService.GenerateCardImageAsync(generateRequest);
            if (!generatedImageResult.IsSuccess)
            {
                return ServiceResult<PreviewCardResponse>.Failure(
                    generatedImageResult.ErrorMessage ?? "Failed to generate image",
                    HttpStatusCode.InternalServerError
                );
            }

            var previewCardResponse = new PreviewCardResponse
            {
                FrontText = generatedContentResult.FrontText ?? string.Empty,
                BackText = generatedContentResult.BackText ?? string.Empty,
                ImageUrl = generatedImageResult.ImageUrl ?? string.Empty,
                ImagePublicId = generatedImageResult.ImagePublicId ?? string.Empty
            };

            return ServiceResult<PreviewCardResponse>.Success(previewCardResponse, HttpStatusCode.Created);
        }

        public async Task<ServiceResult<ICollection<CardResponse>>> GetDueCardsByDeckIdAsync(int deckId, Query? query, ClaimsPrincipal claimsPrincipal)
        {
            var userId = claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return ServiceResult<ICollection<CardResponse>>.Failure(
                    "User not authenticated",
                    HttpStatusCode.Unauthorized
                );
            }

            var deck = await _unitOfWork.DecksRepository.GetByIdAsync(deckId);
            if (deck is null)
            {
                return ServiceResult<ICollection<CardResponse>>.Failure(
                    "Deck not found",
                    HttpStatusCode.NotFound
                );
            }

            if (deck.UserId != userId)
            {
                return ServiceResult<ICollection<CardResponse>>.Failure(
                    "You do not have permission to access this deck",
                    HttpStatusCode.Forbidden
                );
            }

            var dueCards = await _unitOfWork.CardsRepository.GetAllAsync(
                filter: c => c.DeckId == deckId && c.NextReview <= DateTime.UtcNow,
                query: query);

            var CardsResponse = _mapper.Map<ICollection<CardResponse>>(dueCards);

            return ServiceResult<ICollection<CardResponse>>.Success(CardsResponse, HttpStatusCode.OK);
        }

        public async Task<ServiceResult<CardResponse>> ReviewCardAsync(int deckId, int cardId, ReviewRequest reviewRequest, ClaimsPrincipal claimsPrincipal)
        {
            var userId = claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return ServiceResult<CardResponse>.Failure(
                    "User not authenticated",
                    HttpStatusCode.Unauthorized
                );
            }

            var deck = await _unitOfWork.DecksRepository.GetByIdAsync(deckId);
            if (deck is null)
            {
                return ServiceResult<CardResponse>.Failure(
                    "Deck not found",
                    HttpStatusCode.NotFound
                );
            }

            if (deck.UserId != userId)
            {
                return ServiceResult<CardResponse>.Failure(
                    "You do not have permission to access this deck",
                    HttpStatusCode.Forbidden
                );
            }

            var card = await _unitOfWork.CardsRepository.GetByIdAsync(cardId);
            if (card is null || card.DeckId != deckId)
            {
                return ServiceResult<CardResponse>.Failure(
                    "Card not found",
                    HttpStatusCode.NotFound
                );
            }

            if (card.NextReview > DateTime.UtcNow)
            {
                return ServiceResult<CardResponse>.Failure(
                    "Card is not due for review",
                    HttpStatusCode.BadRequest
                );
            }

            var settings = await _unitOfWork.SettingsRepository.GetSettingsByUserIdAsync(userId);
            if (settings is null)
            {
                return ServiceResult<CardResponse>.Failure(
                    "Settings not found",
                    HttpStatusCode.NotFound
                );
            }

            if (!Enum.IsDefined(reviewRequest.Rating))
            {
                return ServiceResult<CardResponse>.Failure(
                    "Invalid review rating",
                    HttpStatusCode.BadRequest
                );
            }

            await _reviewsService.ProcessReview(card, reviewRequest.Rating, settings);

            var cardResponse = _mapper.Map<CardResponse>(card);
            return ServiceResult<CardResponse>.Success(cardResponse, HttpStatusCode.OK);
        }
    }
}
