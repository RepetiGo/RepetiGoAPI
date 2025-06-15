using FlashcardApp.Api.Interfaces.Repositories;

namespace FlashcardApp.Api.Repositories
{
    public class CardsRepository : GenericRepository<Card>, ICardsRepository
    {
        public CardsRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
