using AutoMapper;

using RepetiGo.Api.Dtos.ReviewDtos;
using RepetiGo.Api.Dtos.StatsDtos;

namespace RepetiGo.Api.Services
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

        public async Task ProcessReview(Card card, ReviewRating reviewRating, Settings settings)
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

            // Create a review entry
            var review = new Review
            {
                CardId = card.Id,
                Rating = reviewRating,
                CreatedAt = DateTime.UtcNow
            };
            await _unitOfWork.ReviewsRepository.AddAsync(review);
            await _unitOfWork.SaveAsync();
        }

        private static void ProcessLearningCard(Card card, ReviewRating reviewRating, Settings settings)
        {
            var now = DateTime.UtcNow;
            card.Status = card.Status == CardStatus.New ? CardStatus.Learning : card.Status;
            var steps = ParseSteps(card.Status == CardStatus.Learning ? settings.LearningSteps : settings.RelearningSteps);

            if (reviewRating == ReviewRating.Again) // Reset to the first step
            {
                card.LearningStep = 0;
                if (steps.Count > 0)
                {
                    card.NextReview = now.Add(steps[0]);
                }
                else // No steps defined, fallback to minimum interval
                {
                    card.NextReview = now.AddDays(settings.MinimumInterval);
                }
            }
            else // Process Good, Hard, or Easy ratings
            {
                if (reviewRating == ReviewRating.Easy) // If the user rated the card as Easy
                {
                    card.NextReview = now.AddDays(settings.EasyInterval);
                    card.EasinessFactor += 0.15;
                    card.Status = CardStatus.Review;
                    card.Repetition = 1;
                    card.LearningStep = steps.Count - 1;
                }
                else if (card.LearningStep < steps.Count - 1) // Still in learning steps
                {
                    card.LearningStep++;
                    card.NextReview = now.Add(steps[card.LearningStep]);
                }
                else // Finished learning steps
                {
                    if (card.Status == CardStatus.Relearning) // If the card is in relearning, use the failed interval
                    {
                        card.NextReview = now.AddDays(card.FailedInterval ?? (reviewRating == ReviewRating.Easy ? settings.EasyInterval : settings.GraduatingInterval));
                        card.FailedInterval = null; // Reset the failed interval after using it
                    }
                    else // If the card is in review, set the next review based on the rating
                    {
                        card.NextReview = now.AddDays(settings.GraduatingInterval);
                    }

                    card.Status = CardStatus.Review;
                    card.Repetition = 1;
                    card.LearningStep = steps.Count - 1;
                }
            }
        }

        private static void ProcessReviewCard(Card card, ReviewRating reviewRating, Settings settings)
        {
            var now = DateTime.UtcNow;
            var previousInterval = now - (card.LastReviewed ?? now); // Upgrade from traditional SM-2

            if (reviewRating == ReviewRating.Again) // Reset to relearning
            {
                card.Status = CardStatus.Relearning;
                card.LearningStep = 0;
                card.Repetition = 0;

                // Store the failed interval for later use
                var newIntervalDays = settings.NewInterval * previousInterval.TotalDays;
                newIntervalDays = Math.Clamp(newIntervalDays, settings.MinimumInterval, settings.MaximumInterval);
                card.FailedInterval = newIntervalDays;

                var steps = ParseSteps(settings.RelearningSteps);

                // Reset to the first relearning step
                if (steps.Count > 0)
                {
                    card.NextReview = now.Add(steps[0]);
                }
                else // No steps defined, fallback to minimum interval
                {
                    card.NextReview = now.AddDays(settings.MinimumInterval);
                }
            }
            else // Successful review - SM-2 algorithm
            {
                double newIntervalInDays;
                if (reviewRating == ReviewRating.Hard)
                {
                    newIntervalInDays = previousInterval.TotalDays * settings.HardInterval; // Apply Hard Interval multiplier if applicable
                }
                else
                {
                    if (card.Repetition == 0)
                    {
                        newIntervalInDays = settings.GraduatingInterval;
                    }
                    else if (card.Repetition == 1)
                    {
                        newIntervalInDays = settings.GraduatingInterval * card.EasinessFactor;
                    }
                    else
                    {
                        newIntervalInDays = Math.Round(previousInterval.TotalDays * card.EasinessFactor);
                    }

                    if (reviewRating == ReviewRating.Easy)
                    {
                        newIntervalInDays *= settings.EasyBonus; // Apply Easy Bonus multiplier if applicable
                    }
                }

                newIntervalInDays = Math.Clamp(newIntervalInDays, settings.MinimumInterval, settings.MaximumInterval);

                card.NextReview = now.AddDays(newIntervalInDays);

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

        public ICollection<ReviewResponse> ProcessPreviewReviews(ICollection<Card> cards, Settings settings)
        {
            var reviewResponses = cards.Select(card =>
            {
                var reviewResponse = SimulateReview(card, settings);
                return reviewResponse;
            }).ToList();

            return reviewResponses;
        }

        private ReviewResponse SimulateReview(Card card, Settings settings)
        {
            var now = DateTime.UtcNow;

            var againCard = (Card)card.Clone();
            ProcessPreviewReview(againCard, ReviewRating.Again, settings);
            var againInterval = againCard.NextReview - now;

            var goodCard = (Card)card.Clone();
            ProcessPreviewReview(goodCard, ReviewRating.Good, settings);
            var goodInterval = goodCard.NextReview - now;

            var hardCard = (Card)card.Clone();
            ProcessPreviewReview(hardCard, ReviewRating.Hard, settings);
            var hardInterval = hardCard.NextReview - now;

            var easyCard = (Card)card.Clone();
            ProcessPreviewReview(easyCard, ReviewRating.Easy, settings);
            var easyInterval = easyCard.NextReview - now;

            var reviewResponse = _mapper.Map<ReviewResponse>(card);
            reviewResponse.ReviewTimeResult = new ReviewTimeResult
            {
                Again = FormatTimeSpan(againInterval),
                Good = FormatTimeSpan(goodInterval),
                Hard = FormatTimeSpan(hardInterval),
                Easy = FormatTimeSpan(easyInterval)
            };

            return reviewResponse;
        }

        public void ProcessPreviewReview(Card card, ReviewRating reviewRating, Settings settings)
        {
            if (card.Status == CardStatus.Review)
            {
                ProcessReviewCard(card, reviewRating, settings);
            }
            else
            {
                ProcessLearningCard(card, reviewRating, settings);
            }
        }

        private static string FormatTimeSpan(TimeSpan timeSpan)
        {
            if (timeSpan.TotalSeconds < 60) return $"{Math.Floor(timeSpan.TotalSeconds)}s";
            if (timeSpan.TotalMinutes < 60) return $"{Math.Floor(timeSpan.TotalMinutes)}m";
            if (timeSpan.TotalHours < 24) return $"{Math.Floor(timeSpan.TotalHours)}h";
            if (timeSpan.TotalDays < 30) return $"{Math.Floor(timeSpan.TotalDays)}d";
            if (timeSpan.TotalDays < 365) return $"~{Math.Floor(timeSpan.TotalDays / 30.4)}mo";
            return $"~{Math.Floor(timeSpan.TotalDays / 365.25)}y";
        }

        public async Task<ServiceResult<ActivityStatsResponse>> GetReviewsAsync(int year, ClaimsPrincipal user)
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return ServiceResult<ActivityStatsResponse>.Failure(
                    "User not authenticated",
                    HttpStatusCode.Unauthorized);
            }

            if (year < 2000 || year > DateTime.UtcNow.Year)
            {
                return ServiceResult<ActivityStatsResponse>.Failure(
                    "Year must be between 2000 and the current year",
                    HttpStatusCode.BadRequest);
            }

            //var timeLine = new DateTime(year, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            //var startOfYear = new DateTime(year, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            //var startOfNextYear = new DateTime(year + 1, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            var userReviews = await _unitOfWork.ReviewsRepository.GetAllAsync(
                filter: r => r.Card.Deck.UserId == userId && r.CreatedAt.Year == year
            );

            var userReviewDates = userReviews
                .Select(r => r.CreatedAt)
                .ToList();

            if (userReviewDates.Count == 0)
            {
                return ServiceResult<ActivityStatsResponse>.Failure(
                    "No reviews found for the specified year",
                    HttpStatusCode.NotFound);
            }

            // Calculate the daily review count
            var dailyCount = userReviewDates
                .GroupBy(r => DateOnly.FromDateTime(r))
                .Select(g => new MapPointData
                {
                    Date = g.Key,
                    Count = g.Count()
                })
                .ToList();

            // Create a sorted list of dates for the year
            var learnedDays = dailyCount
                .Select(g => g.Date)
                .OrderBy(d => d)
                .ToHashSet();

            var (longestStreak, currentStreak) = CalculateStreaks(learnedDays);

            var totalDaysSinceStart = DateTime.UtcNow.Year == year
                ? (DateTime.UtcNow - new DateTime(year, 1, 1)).TotalDays
                : (new DateTime(year + 1, 1, 1) - new DateTime(year, 1, 1)).TotalDays;
            var daysLearnedPercent = totalDaysSinceStart > 0 ? (int)Math.Round((double)learnedDays.Count / totalDaysSinceStart * 100) : 0;
            var dailyAverage = learnedDays.Count > 0 ? (int)Math.Round((double)userReviews.Count / learnedDays.Count) : 0;

            return ServiceResult<ActivityStatsResponse>.Success(
                new ActivityStatsResponse
                {
                    DailyReviewCounts = dailyCount,
                    LongestStreak = longestStreak,
                    CurrentStreak = currentStreak,
                    DailyAverage = dailyAverage,
                    DayLearnedPercent = daysLearnedPercent,
                },
                HttpStatusCode.OK
            );
        }

        private (int longestStreak, int currentStreak) CalculateStreaks(HashSet<DateOnly> learnedDays)
        {
            if (learnedDays.Count == 0)
            {
                return (0, 0);
            }

            var longestStreak = 0;
            var currentStreak = 0;

            var sortedDays = learnedDays.OrderBy(d => d).ToList();

            for (var i = 0; i < sortedDays.Count; i++)
            {
                if (i == 0)
                {
                    // Start the first streak
                    currentStreak = 1;
                }
                else
                {
                    if (sortedDays[i] == sortedDays[i - 1].AddDays(1)) // Check if the current day is the next day of the previous
                    {
                        currentStreak++;
                    }
                    else // If the streak is broken
                    {
                        longestStreak = Math.Max(longestStreak, currentStreak);
                        currentStreak = 1;
                    }
                }
            }

            longestStreak = Math.Max(longestStreak, currentStreak);

            // Check if today is included in the learned days
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var yesterday = today.AddDays(-1);

            if (learnedDays.Contains(today) || learnedDays.Contains(yesterday))
            {
                return (longestStreak, currentStreak);
            }
            else
            {
                return (longestStreak, 0);
            }
        }
    }
}
