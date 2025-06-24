namespace RepetiGo.Api.Interfaces.Repositories
{
    public interface IUnitOfWork
    {
        IDecksRepository DecksRepository { get; }
        ICardsRepository CardsRepository { get; }
        IReviewsRepository ReviewsRepository { get; }
        ISettingsRepository SettingsRepository { get; }

        Task<int> SaveAsync();
    }
}