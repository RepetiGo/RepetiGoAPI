using RepetiGo.Api.Interfaces.Repositories;

namespace RepetiGo.Api.Repositories
{
    public class DecksRepository : GenericRepository<Deck>, IDecksRepository
    {
        public DecksRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
