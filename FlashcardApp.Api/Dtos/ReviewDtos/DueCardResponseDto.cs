namespace FlashcardApp.Api.Dtos.ReviewDtos
{
    public class DueCardResponseDto
    {
        public int Id { get; set; }

        public string FrontText { get; set; } = string.Empty;

        public string BackText { get; set; } = string.Empty;

        public CardPreviewResponseDto PreviewDto { get; set; } = new CardPreviewResponseDto();

        public DateTime NextReview { get; set; } = DateTime.UtcNow;

        public int Repetition { get; set; } = 0;

        public int LearningStep { get; set; } = 0;

        public CardStatus Status { get; set; } = CardStatus.New;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // -------------- Navigation properties --------------

        public int DeckId { get; set; }
    }
}
