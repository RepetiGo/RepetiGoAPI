using RepetiGo.Api.Enums;

namespace RepetiGo.Api.Models
{
    public class Review
    {
        public int Id { get; set; }

        public ReviewRating Rating { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // -------------- Navigation properties --------------

        public int CardId { get; set; }

        public Card Card { get; set; } = null!;
    }
}