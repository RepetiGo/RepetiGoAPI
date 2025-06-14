namespace FlashcardApp.Api.ConfigModels
{
    public class ConnectionStringConfig
    {
        public const string SectionName = "ConnectionStrings";

        [Required]
        public required string DefaultConnection { get; set; }

        [Required]
        public required string RedisConnection { get; set; }
    }
}
