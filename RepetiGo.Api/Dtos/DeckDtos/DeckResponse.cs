namespace RepetiGo.Api.Dtos.DeckDtos
{
    public class DeckResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public CardVisibility Visibility { get; set; } = CardVisibility.Public;
        public int Downloads { get; set; } = 0;
        public string? ForkedFromUsername { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string UserId { get; set; } = string.Empty;
    }
}
