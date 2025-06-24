
using RepetiGo.Api.Dtos.ReviewDtos;

namespace RepetiGo.Api.Interfaces.Services
{
    public interface IReviewsService
    {
        ICollection<ReviewResponse> ProcessPreviewReviews(ICollection<Card> cards, Settings settings);
        Task ProcessReview(Card card, ReviewRating reviewRating, Settings settings);
    }
}
