namespace FlashcardApp.Api.Dtos.ReviewDtos
{
    public class ReviewRequest
    {
        [EnumDataType(typeof(ReviewRating))]
        [Range(0, 3, ErrorMessage = "Rating must be between 0 and 3")]
        [Display(Name = "Review Rating")]
        [Required(ErrorMessage = "Rating is required")]
        public ReviewRating Rating { get; set; } = ReviewRating.Again;
    }
}
