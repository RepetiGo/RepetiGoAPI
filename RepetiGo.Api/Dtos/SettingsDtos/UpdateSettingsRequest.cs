namespace RepetiGo.Api.Dtos.SettingsDtos
{
    public class UpdateSettingsRequest
    {
        public int? NewCardsPerDay { get; set; }
        public int? MaxReviewsPerDay { get; set; }
        public double? StartingEasinessFactor { get; set; }
        public string? LearningSteps { get; set; }
        public double? GraduatingInterval { get; set; }
        public double? EasyInterval { get; set; }
        public string? RelearningSteps { get; set; }
        public double? MinimumInterval { get; set; }
        public double? MaximumInterval { get; set; }
        public double? EasyBonus { get; set; }
        public double? HardInterval { get; set; }
        public double? NewInterval { get; set; }
    }
}
