namespace FlashcardApp.Api.Services
{
    public class SettingsService : ISettingsService
    {
        private readonly IUnitOfWork _unitOfWork;

        public SettingsService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> CreateUserSettings(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return false;
            }

            var userSettings = new Settings
            {
                UserId = userId,
            };

            await _unitOfWork.SettingsRepository.AddAsync(userSettings);
            await _unitOfWork.SaveAsync();
            return true;
        }

        public async Task<Settings?> GetSettingsByUserIdAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return null;
            }

            var settings = await _unitOfWork.SettingsRepository.GetAllAsync(
                filter: s => s.UserId == userId);


            return settings.FirstOrDefault();
        }
    }
}
