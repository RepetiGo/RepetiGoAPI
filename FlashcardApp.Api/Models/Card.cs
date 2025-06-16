using RepetiGo.Api.Enums;

namespace RepetiGo.Api.Models
{
    public class Card
    {
        public int Id { get; set; }

        [MaxLength(500)]
        public string FrontText { get; set; } = string.Empty;

        [MaxLength(500)]
        public string BackText { get; set; } = string.Empty;

        // -------------- Spaced Repetition System (SRS) properties --------------

        public DateTime NextReview { get; set; } = DateTime.UtcNow;

        public int Repetition { get; set; } = 0;

        public CardStatus Status { get; set; } = CardStatus.New;

        public double EasinessFactor { get; set; } = 2.5;

        public int LearningStep { get; set; } = 0;

        public string? ImageUrl { get; set; }

        public string? ImagePublicId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // -------------- Navigation properties --------------

        public int DeckId { get; set; }

        public Deck Deck { get; set; } = null!;

        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}