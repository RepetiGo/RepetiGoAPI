using RepetiGo.Api.Interfaces.Repositories;

namespace RepetiGo.Api.Repositories
{
    public class SettingsRepository : GenericRepository<Settings>, ISettingsRepository
    {
        public SettingsRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Settings?> GetSettingsByUserIdAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return null;
            }

            var settings = await _context.Settings.FirstOrDefaultAsync(s => s.UserId == userId);

            return settings;
        }
    }
}
