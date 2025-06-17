namespace RepetiGo.Api.Dtos.CardDtos
{
    public class CardResponse
    {
        public int Id { get; set; }
        public string FrontText { get; set; } = string.Empty;
        public string BackText { get; set; } = string.Empty;

        // -------------- Spaced Repetition System (SRS) properties --------------

        public DateTime NextReview { get; set; } = DateTime.UtcNow;
        public int Repetition { get; set; } = 0;
        public CardStatus Status { get; set; } = CardStatus.New;
        public double EasinessFactor { get; set; } = 2.5;
        public int LearningStep { get; set; } = 0;
        public DateTime? LastReviewed { get; set; }
        public string? ImageUrl { get; set; }
        public string? ImagePublicId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // -------------- Navigation properties --------------

        public int DeckId { get; set; }
    }
}
