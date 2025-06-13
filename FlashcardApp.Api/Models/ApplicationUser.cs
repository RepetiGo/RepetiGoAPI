namespace FlashcardApp.Api.Models
{
    public class ApplicationUser : IdentityUser
    {
        [PersonalData]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [PersonalData]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // -------------- Navigation properties --------------

        public ICollection<Deck> Decks { get; set; } = new List<Deck>();

        public Settings? Settings { get; set; } = null;
    }
}