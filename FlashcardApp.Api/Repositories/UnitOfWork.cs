namespace FlashcardApp.Api.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        private IGenericRepository<Deck>? _decksRepository;

        private IGenericRepository<Card>? _cardsRepository;

        private IGenericRepository<Review>? _reviewsRepository;

        private IGenericRepository<Setting>? _settingsRepository;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public IGenericRepository<Deck> DecksRepository
        {
            get
            {
                return _decksRepository ??= new DecksRepository(_context);
            }
        }

        public IGenericRepository<Card> CardsRepository
        {
            get
            {
                return _cardsRepository ??= new CardsRepository(_context);
            }
        }

        public IGenericRepository<Review> ReviewsRepository
        {
            get
            {
                return _reviewsRepository ??= new ReviewsRepository(_context);
            }
        }

        public IGenericRepository<Setting> SettingsRepository
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
    }
}
