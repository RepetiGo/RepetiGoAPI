namespace FlashcardApp.Api.Dtos.CardDtos
{
    public class CreateCardRequestDto
    {
        [MinLength(1)]
        [MaxLength(500)]
        [RegularExpression(@"^[a-zA-Z0-9\s]+$", ErrorMessage = "Front text must contain only letters, numbers, and spaces")]
        [Display(Name = "Front Text")]
        [Required(ErrorMessage = "Front text is required")]
        public string FrontText { get; set; } = string.Empty;

        [MinLength(1)]
        [MaxLength(500)]
        [RegularExpression(@"^[a-zA-Z0-9\s]+$", ErrorMessage = "Back text must contain only letters, numbers, and spaces")]
        [Display(Name = "Back Text")]
        [Required(ErrorMessage = "Back text is required")]
        public string BackText { get; set; } = string.Empty;
    }
}
