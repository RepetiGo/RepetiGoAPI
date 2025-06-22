namespace RepetiGo.Api.Repositories
{
    public class ReviewsRepository : GenericRepository<Review>, IReviewsRepository
    {
        public ReviewsRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
