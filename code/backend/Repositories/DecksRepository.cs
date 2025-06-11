using backend.Interfaces.Repositories;

namespace backend.Repositories
{
    public class DecksRepository : GenericRepository<Deck>, IGenericRepository<Deck>
    {
        public DecksRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
