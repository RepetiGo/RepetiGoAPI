namespace RepetiGo.Api.Models
{
    public class Settings
    {
        public int Id { get; set; }

        // -------------- Daily Pacing Settings --------------

        public int NewCardsPerDay { get; set; } = 20;

        public int MaxReviewsPerDay { get; set; } = 200;

        // -------------- Algorithm Settings --------------

        public double StartingEasinessFactor { get; set; } = 2.5;

        public int GraduatingInterval { get; set; } = 3;

        public string LearningSteps { get; set; } = "1m 10m";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // -------------- Navigation properties --------------

        public string UserId { get; set; } = string.Empty;

        public ApplicationUser User { get; set; } = null!;
    }
}