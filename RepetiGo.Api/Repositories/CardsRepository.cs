using RepetiGo.Api.Data;
using RepetiGo.Api.Interfaces.Repositories;
using RepetiGo.Api.Models;

namespace RepetiGo.Api.Repositories
{
    public class CardsRepository : GenericRepository<Card>, ICardsRepository
    {
        public CardsRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
