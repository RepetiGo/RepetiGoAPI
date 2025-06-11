namespace FlashcardApp.Api.Models
{
    public class Setting
    {
        [Key]
        public int Id { get; set; }

        // -------------- Daily Pacing Settings --------------

        public int NewCardsPerDay { get; set; } = 20;

        public int MaxReviewsPerDay { get; set; } = 200;

        // -------------- Algorithm Settings --------------

        public double StartingEasinessFactor { get; set; } = 2.5;

        public int LapseIntervalDays { get; set; } = 1;

        public int FirstSuccessInterval { get; set; } = 1;

        public int SecondSuccessInterval { get; set; } = 6;

        public string LearningSteps { get; set; } = "1m 10m";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // -------------- Navigation properties --------------

        [Required]
        public string UserId { get; set; } = string.Empty;

        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; } = null!;
    }
}