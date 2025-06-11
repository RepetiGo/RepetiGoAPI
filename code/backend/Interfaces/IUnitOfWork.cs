using backend.Interfaces.Repositories;

namespace backend.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<Deck> DecksRepository { get; }
        IGenericRepository<Card> CardsRepository { get; }
        IGenericRepository<Review> ReviewsRepository { get; }
        IGenericRepository<Setting> SettingsRepository { get; }

        Task<int> SaveAsync();
    }
}