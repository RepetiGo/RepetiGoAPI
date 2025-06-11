namespace FlashcardApp.Api.Dtos.DeckDtos
{
    public class CreateDeckRequestDto
    {
        [MinLength(1)]
        [MaxLength(100)]
        [RegularExpression(@"^[a-zA-Z0-9\s]+$", ErrorMessage = "Deck name must contain only letters, numbers, and spaces")]
        [Display(Name = "Deck Name")]
        [Required(ErrorMessage = "Deck name is required")]
        public string Name { get; set; } = string.Empty;
    }
}
