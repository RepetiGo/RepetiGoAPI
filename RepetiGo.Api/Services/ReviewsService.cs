namespace RepetiGo.Api.Services
{
    public class ReviewsService : IReviewsService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReviewsService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
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
