using RepetiGo.Api.Enums;

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

        public string ImageUrl { get; set; } = string.Empty;

        public string ImagePublicId { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // -------------- Navigation properties --------------

        public int DeckId { get; set; }
    }
}
