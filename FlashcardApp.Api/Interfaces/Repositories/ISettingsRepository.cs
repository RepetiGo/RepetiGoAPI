using RepetiGo.Api.Models;

namespace RepetiGo.Api.Interfaces.Repositories
{
    public interface ISettingsRepository : IGenericRepository<Settings>
    {
        Task<Settings?> GetSettingsByUserIdAsync(string userId);
    }
}
