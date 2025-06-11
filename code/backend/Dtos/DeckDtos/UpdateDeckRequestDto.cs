namespace backend.Dtos.DeckDtos
{
    public class UpdateDeckRequestDto
    {
        [MinLength(1)]
        [MaxLength(100)]
        [RegularExpression(@"^[a-zA-Z0-9\s]+$", ErrorMessage = "Deck name can only contain letters, numbers, and spaces.")]
        [Display(Name = "Deck Name")]
        [StringLength(100, ErrorMessage = "Deck name cannot exceed 100 characters.")]
        public string? Name { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;
    }
}
