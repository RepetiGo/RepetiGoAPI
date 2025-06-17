using RepetiGo.Api.Interfaces.Repositories;

namespace RepetiGo.Api.Repositories
{
    public class CardsRepository : GenericRepository<Card>, ICardsRepository
    {
        public CardsRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
