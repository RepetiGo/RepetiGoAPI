namespace FlashcardApp.Api.Interfaces
{
    public interface IUnitOfWork
    {
        IGenericRepository<Deck> DecksRepository { get; }
        IGenericRepository<Card> CardsRepository { get; }
        IGenericRepository<Review> ReviewsRepository { get; }
        IGenericRepository<Settings> SettingsRepository { get; }

        Task<int> SaveAsync();
    }
}