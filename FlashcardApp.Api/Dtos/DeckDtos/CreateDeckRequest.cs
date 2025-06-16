using RepetiGo.Api.Enums;

namespace RepetiGo.Api.Dtos.DeckDtos
{
    public class CreateDeckRequest
    {
        [MinLength(1)]
        [MaxLength(100)]
        [Display(Name = "Deck Name")]
        [Required(ErrorMessage = "Deck name is required")]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        [Display(Name = "Description")]
        public string Description { get; set; } = string.Empty;

        [Display(Name = "Visibility")]
        [Required(ErrorMessage = "Visibility is required")]
        [EnumDataType(typeof(CardVisibility), ErrorMessage = "Invalid visibility type")]
        public CardVisibility Visibility { get; set; } = CardVisibility.Public;
    }
}
