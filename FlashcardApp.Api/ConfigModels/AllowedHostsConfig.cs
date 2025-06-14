namespace FlashcardApp.Api.ConfigModels
{
    public class AllowedHostsConfig
    {
        public const string SectionName = "AllowedHosts";

        [Required]
        public required string AllowedHosts { get; set; }
    }
}
