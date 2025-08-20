namespace RepetiGo.Api.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        private IDecksRepository? _decksRepository;

        private ICardsRepository? _cardsRepository;

        private IReviewsRepository? _reviewsRepository;

        private ISettingsRepository? _settingsRepository;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public IDecksRepository DecksRepository
        {
            get
            {
                return _decksRepository ??= new DecksRepository(_context);
            }
        }

        public ICardsRepository CardsRepository
        {
            get
            {
                return _cardsRepository ??= new CardsRepository(_context);
            }
        }

        public IReviewsRepository ReviewsRepository
        {
            get
            {
                return _reviewsRepository ??= new ReviewsRepository(_context);
            }
        }

        public ISettingsRepository SettingsRepository
        {
            get
            {
                return _settingsRepository ??= new SettingsRepository(_context);
            }
        }

        public virtual async Task<int> SaveAsync()
        {
            return await _context.SaveChangesAsync();
        }

        // Unit of work doesn't create its own context so we don't need to dispose of it, it depends on the DI container, which decides based on the lifetime we registered the UnitOfWork as a service
    }
}
