namespace RepetiGo.Api.Models
{
    public class Settings
    {
        public int Id { get; set; }

        // -------------- Daily Pacing Settings --------------

        public int NewCardsPerDay { get; set; } = 20;

        public int MaxReviewsPerDay { get; set; } = 200;

        // -------------- Algorithm Settings --------------

        /// <summary>
        /// The ease multiplier new cards start with.
        /// By default, the Good button on a newly-learned card will delay the next review by 2.5x the previous delay.
        /// </summary>
        public double StartingEasinessFactor { get; set; } = 2.5;

        /// <summary>
        /// One or more delays, separated by spaces.
        /// The first delay will be used when you press the Again button on a new card, and is 1 minute by default.
        /// The Good button will advance to the next step, which is 10 minutes by default.
        /// Once all steps have been passed, the card will become a review card, and will appear on a different day.
        /// Delays are typically minutes (e.g. 1m) or days (e.g. 2d), but hours (e.g. 1h)
        /// </summary>
        public string LearningSteps { get; set; } = "25m 1d";

        /// <summary>
        /// The number of days to wait before showing a card again, after the Good button is pressed on the final learning step.
        /// </summary>
        public double GraduatingInterval { get; set; } = 3;

        /// <summary>
        /// The number of days to wait before showing a card again, after the Easy button is used to immediately remove a card from learning.
        /// </summary>
        public double EasyInterval { get; set; } = 4;

        /// <summary>
        /// Zero or more delays, separated by spaces.
        /// By default, pressing the Again button on a review card will show it again 10 minutes later.
        /// If no delays are provided, the card will have its interval changed, without entering relearning.
        /// Delays are typically minutes (e.g. 1m) or days (e.g. 2d), but hours (e.g. 1h) and seconds (e.g. 30s) are also supported.
        /// </summary>
        public string RelearningSteps { get; set; } = "30m";

        /// <summary>
        /// The minimum interval given to a review card after answering Again.
        /// </summary>
        public double MinimumInterval { get; set; } = 1;

        /// <summary>
        /// The maximum number of days a review card will wait.
        /// When reviews have reached the limit, Hard, Good and Easy will all give the same delay.
        /// </summary>
        public double MaximumInterval { get; set; } = 180;

        /// <summary>
        /// The multiplier applied to a interval when answering Hard.
        /// </summary>
        public double HardInterval { get; set; } = 1.2;

        /// <summary>
        /// The multiplier when the user selects "Again" during a review.
        /// The default value is 0.2, which means the next review will be scheduled 20% of the current interval.
        /// If current interval * NewInterval is less than the minimum interval, the minimum interval will be used instead.
        /// </summary>
        public double NewInterval { get; set; } = 0.2;

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        // -------------- Navigation properties --------------

        public string UserId { get; set; } = string.Empty;

        public ApplicationUser User { get; set; } = null!;
    }
}