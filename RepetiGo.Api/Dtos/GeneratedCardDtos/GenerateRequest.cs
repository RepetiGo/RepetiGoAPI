using System.ComponentModel;

using Mscc.GenerativeAI;

namespace RepetiGo.Api.Dtos.GeneratedCardDtos
{
    public class GenerateRequest
    {
        [Required(ErrorMessage = "Topic is required.")]
        [MaxLength(100, ErrorMessage = "Topic cannot exceed 100 characters.")]
        [MinLength(1, ErrorMessage = "Topic must be at least 1 character long.")]
        [Display(Name = "Topic")]
        [Description("The topic for which flashcards will be generated.")]
        public string Topic { get; set; } = string.Empty;

        public string? FrontText { get; set; }

        public string? BackText { get; set; }

        [EnumDataType(typeof(ImagePromptLanguage), ErrorMessage = "Invalid image prompt language.")]
        public ImagePromptLanguage ImagePromptLanguage { get; set; } = ImagePromptLanguage.Auto;

        public bool EnhancePrompt { get; set; } = false;

        public string AspectRatio { get; set; } = "16:9";
    }
}
