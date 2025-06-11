using FlashcardApp.Api.Data;
using FlashcardApp.Api.Models;

namespace FlashcardApp.Api.Repositories
{
    public class ReviewsRepository : GenericRepository<Review>, IGenericRepository<Review>
    {
        public ReviewsRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
