using FlashcardApp.Api.Interfaces.Repositories;

namespace FlashcardApp.Api.Interfaces
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