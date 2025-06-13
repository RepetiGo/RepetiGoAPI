namespace FlashcardApp.Api.Dtos.DeckDtos
{
    public class DeckResponseDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; } = null;

        public CardVisibility Visibility { get; set; } = CardVisibility.Public;

        public int Ratings { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // -------------- Navigation properties --------------

        public string UserId { get; set; } = string.Empty;
    }
}
