using AutoMapper;

using RepetiGo.Api.Dtos.CardDtos;
using RepetiGo.Api.Dtos.GeneratedCardDtos;
using RepetiGo.Api.Dtos.ReviewDtos;
using RepetiGo.Api.Interfaces;

namespace RepetiGo.Api.Services
{
    public class CardsService : ICardsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUploadsService _uploadsService;
        private readonly ILogger<CardsService> _logger;
        private readonly IAiGeneratorService _aiGeneratorService;

        public CardsService(IUnitOfWork unitOfWork, IMapper mapper, IUploadsService uploadsService, ILogger<CardsService> logger, IAiGeneratorService aiGeneratorService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _uploadsService = uploadsService;
            _logger = logger;
            _aiGeneratorService = aiGeneratorService;
        }

        public async Task<ServiceResult<ICollection<CardResponse>>> GetCardsByDeckIdAsync(int deckId, PaginationQuery? paginationQuery, ClaimsPrincipal claimsPrincipal)
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

            var cards = await _unitOfWork.CardsRepository.GetAllAsync(
                filter: c => c.DeckId == deckId,
                orderBy: q => q.OrderBy(c => c.CreatedAt),
                paginationQuery: paginationQuery);

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
                        uploadResult.ErrorMessage,
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
                FrontText = generatedContentResult.FrontText,
                BackText = generatedContentResult.BackText,
                ImageUrl = generatedImageResult.ImageUrl,
                ImagePublicId = generatedImageResult.ImagePublicId
            };

            return ServiceResult<PreviewCardResponse>.Success(previewCardResponse, HttpStatusCode.Created);
        }

        public async Task<ServiceResult<ICollection<CardResponse>>> GetDueCardsByDeckIdAsync(int deckId, PaginationQuery? paginationQuery, ClaimsPrincipal claimsPrincipal)
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
                orderBy: q => q.OrderBy(c => c.NextReview),
                paginationQuery: paginationQuery);

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

            await ProcessReview(card, reviewRequest.Rating, settings);

            var cardResponse = _mapper.Map<CardResponse>(card);
            return ServiceResult<CardResponse>.Success(cardResponse, HttpStatusCode.OK);
        }

        private async Task ProcessReview(Card card, ReviewRating reviewRating, Settings settings)
        {
            if (card.Status == CardStatus.Review)
            {
                ProcessReviewCard(card, reviewRating, settings);
            }
            else
            {
                ProcessLearningCard(card, reviewRating, settings);
            }

            // Update the card
            card.LastReviewed = DateTime.UtcNow;
            await _unitOfWork.CardsRepository.UpdateAsync(card);
            await _unitOfWork.SaveAsync();
        }

        private void ProcessLearningCard(Card card, ReviewRating reviewRating, Settings settings)
        {
            card.Status = card.Status == CardStatus.New ? CardStatus.Learning : card.Status;
            var steps = ParseSteps(card.Status == CardStatus.Learning ? settings.LearningSteps : settings.RelearningSteps);

            if (reviewRating == ReviewRating.Again) // Reset to the first step
            {
                card.LearningStep = 0;
                if (steps.Count > 0)
                {
                    card.NextReview = DateTime.UtcNow.Add(steps[0]);
                }
                else // No steps defined, fallback to minimum interval
                {
                    card.NextReview = DateTime.UtcNow.AddDays(settings.MinimumInterval);
                }
            }
            else if (reviewRating == ReviewRating.Good || reviewRating == ReviewRating.Hard)
            {
                card.LearningStep++;

                if (card.LearningStep < steps.Count)
                {
                    card.NextReview = DateTime.UtcNow.Add(steps[card.LearningStep] * (reviewRating == ReviewRating.Hard ? settings.HardInterval : 1));
                }
                else
                {
                    // Graduation
                    card.Status = CardStatus.Review;
                    card.Repetition = 0;
                    card.NextReview = DateTime.UtcNow.AddDays(settings.GraduatingInterval * (reviewRating == ReviewRating.Hard ? settings.HardInterval : 1));
                }
            }
            else if (reviewRating == ReviewRating.Easy)
            {
                card.Status = CardStatus.Review;
                card.Repetition = 0;
                card.NextReview = DateTime.UtcNow.AddDays(settings.EasyInterval);
            }
        }

        private void ProcessReviewCard(Card card, ReviewRating reviewRating, Settings settings)
        {
            if (reviewRating == ReviewRating.Again) // Switch to relearning mode
            {
                card.Status = CardStatus.Relearning;
                card.LearningStep = 0;
                card.Repetition = 0;

                // Reschedule before entering the first relearning step by recalculating the final timestamp
                var previousInterval = DateTime.UtcNow - (card.LastReviewed ?? DateTime.UtcNow); // Upgrade from tranditional SM-2

                var newIntervalDays = settings.NewInterval * previousInterval;

                // Calculate the next review time based on the settings
                if (settings.MinimumInterval > 0 && newIntervalDays < TimeSpan.FromDays(settings.MinimumInterval))
                {
                    newIntervalDays = TimeSpan.FromDays(settings.MinimumInterval);
                }
                else if (settings.MaximumInterval > 0 && newIntervalDays > TimeSpan.FromDays(settings.MaximumInterval))
                {
                    newIntervalDays = TimeSpan.FromDays(settings.MaximumInterval);
                }

                card.NextReview = DateTime.UtcNow.Add(newIntervalDays);
            }
            else // Successful review - Standard SM-2 logic
            {
                // Calculate a new interval in days
                double newIntervalInDays;
                if (card.Repetition == 0)
                {
                    newIntervalInDays = settings.GraduatingInterval;
                }
                else if (card.Repetition == 1)
                {
                    newIntervalInDays = settings.GraduatingInterval * 2;
                }
                else
                {
                    var previousInterval = DateTime.UtcNow - (card.LastReviewed ?? DateTime.UtcNow); // Upgrade from traditional SM-2
                    newIntervalInDays = Math.Round(previousInterval.TotalDays * card.EasinessFactor);
                }
                newIntervalInDays = Math.Clamp(newIntervalInDays, settings.MinimumInterval, settings.MaximumInterval);

                if (reviewRating == ReviewRating.Hard)
                {
                    newIntervalInDays *= settings.HardInterval; // Apply Hard Interval multiplier if applicable
                }

                card.NextReview = DateTime.UtcNow.AddDays(newIntervalInDays);

                // Update SRS state
                card.Repetition++;
            }

            // Update Easiness Factor
            var q = (int)reviewRating + 2;
            var newEasinessFactor = card.EasinessFactor + (0.1 - (5 - q) * (0.08 + (5 - q) * 0.02));
            card.EasinessFactor = Math.Max(1.3, newEasinessFactor);
        }

        private static List<TimeSpan> ParseSteps(string learningSteps)
        {
            var steps = new List<TimeSpan>();
            var parts = learningSteps.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            foreach (var part in parts)
            {
                var value = int.Parse(part.Substring(0, part.Length - 1));
                var unit = part.Last();
                switch (unit)
                {
                    case 'm': // minutes
                        steps.Add(TimeSpan.FromMinutes(value));
                        break;
                    case 'h': // hours
                        steps.Add(TimeSpan.FromHours(value));
                        break;
                    case 'd': // days
                        steps.Add(TimeSpan.FromDays(value));
                        break;
                    default:
                        throw new FormatException($"Invalid time unit: {unit}");
                }
            }

            return steps;
        }
    }
}
