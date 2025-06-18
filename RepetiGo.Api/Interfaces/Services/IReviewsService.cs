namespace RepetiGo.Api.Interfaces.Services
{
    public interface IReviewsService
    {
        Task ProcessReview(Card card, ReviewRating reviewRating, Settings settings);
    }
}
