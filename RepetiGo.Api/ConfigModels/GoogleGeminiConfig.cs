namespace RepetiGo.Api.ConfigModels
{
    public class GoogleGeminiConfig
    {
        public const string SectionName = "GoogleGemini";

        [Required]
        public required string ProjectId { get; set; }
    }
}
