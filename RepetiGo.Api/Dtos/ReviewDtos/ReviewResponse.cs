namespace RepetiGo.Api.Dtos.ReviewDtos
{
    public class ReviewResponse
    {
        public int Id { get; set; }
        public string FrontText { get; set; } = string.Empty;
        public string BackText { get; set; } = string.Empty;
        public DateTime NextReview { get; set; } = DateTime.UtcNow;
        public ReviewTimeResult ReviewTimeResult { get; set; } = new ReviewTimeResult();
        public int Repetition { get; set; } = 0;
        public CardStatus Status { get; set; } = CardStatus.New;
        public double EasinessFactor { get; set; } = 2.5;
        public int LearningStep { get; set; } = 0;
        public DateTime? LastReviewed { get; set; }
        public string? ImageUrl { get; set; }
        public string? ImagePublicId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int DeckId { get; set; }
    }
}
