using RepetiGo.Api.Data;
using RepetiGo.Api.Interfaces.Repositories;
using RepetiGo.Api.Models;

namespace RepetiGo.Api.Repositories
{
    public class DecksRepository : GenericRepository<Deck>, IDecksRepository
    {
        public DecksRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
