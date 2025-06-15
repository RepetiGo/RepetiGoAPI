using FlashcardApp.Api.Interfaces.Repositories;

namespace FlashcardApp.Api.Repositories
{
    public class DecksRepository : GenericRepository<Deck>, IDecksRepository
    {
        public DecksRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
