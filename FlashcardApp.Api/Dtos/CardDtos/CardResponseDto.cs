namespace FlashcardApp.Api.Dtos.CardDtos
{
    public class CardResponseDto
    {
        public int Id { get; set; }

        public string FrontText { get; set; } = string.Empty;

        public string BackText { get; set; } = string.Empty;

        public DateTime NextReview { get; set; } = DateTime.UtcNow;

        public int Repetition { get; set; } = 0;

        public CardStatus Status { get; set; } = CardStatus.New;

        public int LearningStep { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // -------------- Navigation properties --------------

        public int DeckId { get; set; }
    }
}
