namespace RepetiGo.Api.Dtos.DeckDtos
{
    public class DeckResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public CardVisibility Visibility { get; set; } = CardVisibility.Public;
        public int Ratings { get; set; } = 0;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // -------------- Navigation properties --------------

        public string UserId { get; set; } = string.Empty;
    }
}
