using FlashcardApp.Api.Data;
using FlashcardApp.Api.Models;

namespace FlashcardApp.Api.Repositories
{
    public class DecksRepository : GenericRepository<Deck>, IGenericRepository<Deck>
    {
        public DecksRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
