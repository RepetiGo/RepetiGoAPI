using RepetiGo.Api.Enums;

namespace RepetiGo.Api.Dtos.DeckDtos
{
    public class DeckResponse
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public CardVisibility Visibility { get; set; } = CardVisibility.Public;

        public int Ratings { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // -------------- Navigation properties --------------

        public string UserId { get; set; } = string.Empty;
    }
}
