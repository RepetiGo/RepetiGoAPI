namespace RepetiGo.Api.ConfigModels
{
    public class CloudinaryConfig
    {
        public const string SectionName = "Cloudinary";

        [Required]
        public required string CloudName { get; set; }

        [Required]
        public required string ApiKey { get; set; }

        [Required]
        public required string ApiSecret { get; set; }

        [Required]
        public required string UploadPreset { get; set; }
    }
}
