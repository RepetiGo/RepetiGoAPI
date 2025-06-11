using backend.Interfaces.Repositories;

namespace backend.Repositories
{
    public class SettingsRepository : GenericRepository<Setting>, IGenericRepository<Setting>
    {
        public SettingsRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
