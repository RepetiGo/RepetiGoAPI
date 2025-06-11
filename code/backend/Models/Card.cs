namespace backend.Models
{
    public class Card
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string FrontText { get; set; } = string.Empty;

        [Required]
        public string BackText { get; set; } = string.Empty;

        public DateTime? NextReview { get; set; }
        public int Repetition { get; set; } = 0;
        public int IntervalDays { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public int DeckId { get; set; }

        [ForeignKey("DeckId")]
        public Deck Deck { get; set; } = null!;
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}