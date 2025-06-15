namespace FlashcardApp.Api.Interfaces.Repositories
{
    public interface ISettingsRepository : IGenericRepository<Settings>
    {
        Task<Settings?> GetSettingsByUserIdAsync(string userId);
    }
}
