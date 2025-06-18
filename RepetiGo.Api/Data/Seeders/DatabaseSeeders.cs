using Bogus;

namespace RepetiGo.Api.Data.Seeders
{
    public class DatabaseSeeders
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public DatabaseSeeders(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task SeedDatabaseAsync()
        {
            if (!_context.Decks.Any())
            {
                var fakeUser = await GenerateFakeUsers(10);
                if (fakeUser is not null)
                {
                    var fakeDecks = GenerateFakeDecks(50, fakeUser);
                    await _context.Decks.AddRangeAsync(fakeDecks);
                    await _context.SaveChangesAsync();

                    var fakeCards = GenerateFakeCards(500, fakeDecks);
                    await _context.Cards.AddRangeAsync(fakeCards);
                    await _context.SaveChangesAsync();

                    var fakeReviews = GenerateFakeReviews(1000, fakeCards);
                    await _context.Reviews.AddRangeAsync(fakeReviews);
                    await _context.SaveChangesAsync();
                }
            }
        }

        private static ICollection<Deck> GenerateFakeDecks(int count, ApplicationUser user)
        {
            var faker = new Faker<Deck>()
                .RuleFor(d => d.Name, f => f.Commerce.ProductName())
                .RuleFor(d => d.Description, f => f.Lorem.Sentence())
                .RuleFor(d => d.Visibility, f => f.PickRandom<CardVisibility>())
                .RuleFor(d => d.CreatedAt, f => f.Date.Past(1))
                .RuleFor(d => d.UpdatedAt, f => f.Date.Recent())
                .RuleFor(d => d.UserId, f => f.PickRandom(user).Id);

            return faker.Generate(count);
        }

        private static ICollection<Card> GenerateFakeCards(int count, ICollection<Deck> decks)
        {
            var faker = new Faker<Card>()
                .RuleFor(c => c.FrontText, f => f.Lorem.Sentence())
                .RuleFor(c => c.BackText, f => f.Lorem.Sentence())
                .RuleFor(c => c.DeckId, f => f.PickRandom(decks).Id)
                .RuleFor(c => c.NextReview, DateTime.UtcNow)
                .RuleFor(c => c.ImageUrl, f => f.Image.PicsumUrl())
                .RuleFor(c => c.ImagePublicId, f => f.Random.AlphaNumeric(10))
                .RuleFor(c => c.CreatedAt, f => f.Date.Past(1))
                .RuleFor(c => c.UpdatedAt, f => f.Date.Recent());

            return faker.Generate(count);
        }

        private async Task<ApplicationUser?> GenerateFakeUsers(int count)
        {
            var fakeUserEntity = new ApplicationUser()
            {
                Email = "fakeemail@example.com",
                NormalizedEmail = "FAKEEMAIL@EXAMPLE.COM",
                UserName = "fakeuser",
                NormalizedUserName = "FAKEUSER",
                EmailConfirmed = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(fakeUserEntity, "String123@");
            if (result.Succeeded)
            {
                var fakeSettings = new Settings
                {
                    UserId = fakeUserEntity.Id,
                    CreatedAt = DateTime.UtcNow,
                };

                await _context.Settings.AddAsync(fakeSettings);
                await _context.SaveChangesAsync();

                return fakeUserEntity;
            }

            return null;
        }

        private static ICollection<Review> GenerateFakeReviews(int count, ICollection<Card> cards)
        {
            var faker = new Faker<Review>()
                .RuleFor(r => r.CardId, f => f.PickRandom(cards).Id)
                .RuleFor(r => r.Rating, f => f.PickRandom<ReviewRating>())
                .RuleFor(r => r.CreatedAt, f => f.Date.Past(1))
                .RuleFor(r => r.CardId, f => f.PickRandom(cards).Id);

            return faker.Generate(count);
        }
    }
}
