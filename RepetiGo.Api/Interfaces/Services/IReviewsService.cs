

using RepetiGo.Api.Dtos.ReviewDtos;
using RepetiGo.Api.Dtos.StatsDtos;

namespace RepetiGo.Api.Interfaces.Services
{
    public interface IReviewsService
    {
        Task<ServiceResult<ActivityStatsResponse>> GetReviewsAsync(int year, ClaimsPrincipal user);
        ICollection<ReviewResponse> ProcessPreviewReviews(ICollection<Card> cards, Settings settings);
        Task ProcessReview(Card card, ReviewRating reviewRating, Settings settings);
    }
}
