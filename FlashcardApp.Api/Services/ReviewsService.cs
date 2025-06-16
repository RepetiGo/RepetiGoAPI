using AutoMapper;

using FlashcardApp.Api.Dtos.CardDtos;
using FlashcardApp.Api.Dtos.ReviewDtos;
using FlashcardApp.Api.Interfaces;

namespace FlashcardApp.Api.Services
{
    public class ReviewsService : IReviewsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ReviewsService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
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
            if (card.Status == CardStatus.New || card.Status == CardStatus.Learning)
            {
                ProcessLearning(card, reviewRating, settings);
            }
            else
            {
                ProcessReviewCard(card, reviewRating, settings);
            }

            // Update the card
            await _unitOfWork.CardsRepository.UpdateAsync(card);
            await _unitOfWork.SaveAsync();
        }

        private void ProcessReviewCard(Card card, ReviewRating reviewRating, Settings settings)
        {
            card.Status = CardStatus.Learning;
            var learningSteps = ParseSteps(settings.LearningSteps);

            if (reviewRating == ReviewRating.Again) // Reset to the first step
            {
                card.LearningStep = 0;
                card.NextReview = DateTime.UtcNow.Add(learningSteps[0]);
            }
            else if (reviewRating == ReviewRating.Good)
            {
                card.LearningStep++;

                if (card.LearningStep < learningSteps.Count())
                {
                    card.NextReview = DateTime.UtcNow.Add(learningSteps[card.LearningStep]);
                }
                else
                {
                    // Graduation
                    card.Status = CardStatus.Review;
                    card.Repetition = 0;
                    card.NextReview = DateTime.UtcNow.AddDays(settings.GraduatingInterval);
                }
            }
            //else if (reviewRating == ReviewRating.Easy)
            //{
            //    card.Status = CardStatus.Review;
            //    card.Repetition = 0;
            //    card.NextReview = DateTime.UtcNow.AddDays(settings.GraduatingInterval);
            //}
        }

        private void ProcessLearning(Card card, ReviewRating reviewRating, Settings settings)
        {
            if (reviewRating == ReviewRating.Again) // This is a LAPSE
            {
                card.Status = CardStatus.Learning;
                card.LearningStep = 0;
                card.Repetition = 0;
                card.EasinessFactor = Math.Max(1.3, card.EasinessFactor - 0.2);

                // Reschedule to the first learning step by recalculating the final timestamp
                var firstStep = ParseSteps(settings.LearningSteps).First();
                card.NextReview = DateTime.UtcNow.Add(firstStep);
            }
            else // Successful review - Standard SM-2 logic
            {
                // Calculate a new interval in days
                int newIntervalInDays;
                if (card.Repetition == 0)
                {
                    newIntervalInDays = settings.GraduatingInterval;
                }
                else
                {
                    TimeSpan previousInterval = DateTime.UtcNow - card.NextReview;
                    newIntervalInDays = (int)Math.Round(previousInterval.TotalDays * card.EasinessFactor);
                }

                // Update SRS state
                card.Repetition++;

                // Update Easiness Factor
                var q = (int)reviewRating + 2;
                double newEasinessFactor = card.EasinessFactor + (0.1 - (5 - q) * (0.08 + (5 - q) * 0.02));
                card.EasinessFactor = Math.Max(1.3, newEasinessFactor);

                // Set the next review date
                card.NextReview = DateTime.UtcNow.AddDays(newIntervalInDays);
                card.UpdatedAt = DateTime.UtcNow;
            }
        }

        private List<TimeSpan> ParseSteps(string learningSteps)
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
