namespace FlashcardApp.Api.Dtos.CardDtos
{
    public class CreateCardRequestDto
    {
        [Required]
        [MinLength(1)]
        [MaxLength(500)]
        public string FrontText { get; set; } = string.Empty;

        [Required]
        [MinLength(1)]
        [MaxLength(500)]
        public string BackText { get; set; } = string.Empty;
    }
}
