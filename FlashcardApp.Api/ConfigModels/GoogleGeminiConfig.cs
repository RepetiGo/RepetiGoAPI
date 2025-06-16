namespace FlashcardApp.Api.ConfigModels
{
    public class GoogleGeminiConfig
    {
        public const string SectionName = "GoogleGemini";

        [Required]
        public required string ApiKey { get; set; }

        [Required]
        public required string ProjectId { get; set; }

        //[Required]
        //public required string AccessToken { get; set; }

        [Required]
        public required string Region { get; set; }
    }
}
