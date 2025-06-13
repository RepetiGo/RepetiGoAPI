namespace FlashcardApp.Api.Models
{
    public class Deck
    {
        public int Id { get; set; }

        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; } = null;

        public CardVisibility Visibility { get; set; } = CardVisibility.Public;

        public int Ratings { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // -------------- Navigation properties --------------

        public string UserId { get; set; } = string.Empty;

        public ApplicationUser User { get; set; } = null!;

        public ICollection<Card> Cards { get; set; } = new List<Card>();
    }
}