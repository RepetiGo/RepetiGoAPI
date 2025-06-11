namespace backend.Dtos.DeckDtos
{
    public class DeckDto
    {
        [Required]
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public string UserId { get; set; } = string.Empty;

        public ApplicationUser User { get; set; } = null!;

        public ICollection<CardDto> Cards { get; set; } = new List<CardDto>();
    }
}
