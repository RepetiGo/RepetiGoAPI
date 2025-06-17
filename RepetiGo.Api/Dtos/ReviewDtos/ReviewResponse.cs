namespace RepetiGo.Api.Dtos.ReviewDtos
{
    public class ReviewResponse
    {
        public int Id { get; set; }
        public ReviewRating Rating { get; set; }
        public DateTime CreatedAt { get; set; }

        // -------------- Navigation properties --------------

        public int CardId { get; set; }
        public Card Card { get; set; } = null!;
    }
}
