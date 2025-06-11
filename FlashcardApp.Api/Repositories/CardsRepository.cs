namespace FlashcardApp.Api.Repositories
{
    public class CardsRepository : GenericRepository<Card>, IGenericRepository<Card>
    {
        public CardsRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
