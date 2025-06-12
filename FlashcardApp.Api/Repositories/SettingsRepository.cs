using FlashcardApp.Api.Data;
using FlashcardApp.Api.Models;

namespace FlashcardApp.Api.Repositories
{
    public class SettingsRepository : GenericRepository<Settings>, IGenericRepository<Settings>
    {
        public SettingsRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
