namespace FlashcardApp.Api.ConfigModels
{
    public class JwtConfig
    {
        public const string SectionName = "Jwt";

        [Required]
        public required string Audience { get; set; }

        [Required]
        public required string Issuer { get; set; }

        [Required]
        public required string Secret { get; set; }

        [Required]
        public required int TokenValidityInMinutes { get; set; }

        [Required]
        public required int RefreshTokenValidityInDays { get; set; }
    }
}
