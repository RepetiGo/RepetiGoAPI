namespace RepetiGo.Api.Dtos.SettingsDtos
{
    public class SettingsResponse
    {
        public int Id { get; set; }
        public int NewCardsPerDay { get; set; }
        public int MaxReviewsPerDay { get; set; }
        public double StartingEasinessFactor { get; set; }
        public string LearningSteps { get; set; } = string.Empty;
        public double GraduatingInterval { get; set; }
        public double EasyInterval { get; set; }
        public string RelearningSteps { get; set; } = string.Empty;
        public double MinimumInterval { get; set; }
        public double MaximumInterval { get; set; }
        public double HardInterval { get; set; }
        public double NewInterval { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // -------------- Navigation properties --------------

        public string UserId { get; set; } = string.Empty;
    }
}
