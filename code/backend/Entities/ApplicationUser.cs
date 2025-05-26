namespace backend.Entities
{
    public class ApplicationUser : IdentityUser
    {
        [PersonalData]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [PersonalData]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Deck> Decks { get; set; } = new List<Deck>();
        public ICollection<Setting> Settings { get; set; } = new List<Setting>();
    }
}
