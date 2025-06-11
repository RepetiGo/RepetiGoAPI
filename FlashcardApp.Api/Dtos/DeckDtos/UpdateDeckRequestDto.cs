namespace FlashcardApp.Api.Dtos.DeckDtos
{
    public class UpdateDeckRequestDto
    {
        [MinLength(1)]
        [MaxLength(100)]
        [Display(Name = "Deck Name")]
        [StringLength(100, ErrorMessage = "Deck name cannot exceed 100 characters.")]
        public string? Name { get; set; }
    }
}
