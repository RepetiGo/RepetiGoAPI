namespace RepetiGo.Api.ConfigModels
{
    public class ClientConfig
    {
        public const string SectionName = "Client";

        [Required]
        public required string BaseUrl { get; set; }

        [Required]
        public required string WebAppUrl { get; set; }

        [Required]
        public required string MobileAppUrl { get; set; }
    }
}
