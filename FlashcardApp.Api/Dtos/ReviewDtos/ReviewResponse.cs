namespace FlashcardApp.Api.Dtos.ReviewDtos
{
    public class ReviewResponse
    {
        public ReviewRating Rating { get; set; }

        // -------------- Navigation properties --------------

        public int CardId { get; set; }

        public Card Card { get; set; } = null!;
    }
}
