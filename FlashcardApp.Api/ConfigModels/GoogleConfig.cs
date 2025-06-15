namespace FlashcardApp.Api.ConfigModels
{
    public class GoogleConfig
    {
        public const string SectionName = "Google";

        [Required]
        public required string ClientId { get; set; }

        [Required]
        public required string ClientSecret { get; set; }

        [Required]
        public required string RedirectUri { get; set; }
    }
}
