using backend.Interfaces.Repositories;

namespace backend.Repositories
{
    public class ReviewsRepository : GenericRepository<Review>, IGenericRepository<Review>
    {
        public ReviewsRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
