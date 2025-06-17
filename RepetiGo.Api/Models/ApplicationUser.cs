namespace RepetiGo.Api.Models
{
    public class ApplicationUser : IdentityUser
    {
        [PersonalData]
        public string? AvatarUrl { get; set; }

        [PersonalData]
        public string? AvatarPublicId { get; set; }

        [PersonalData]
        public DateTime CreatedAt { get; set; }

        [PersonalData]
        public DateTime UpdatedAt { get; set; }

        // -------------- Navigation properties --------------

        public ICollection<Deck> Decks { get; set; } = new List<Deck>();

        public Settings? Settings { get; set; } = null;
    }
}