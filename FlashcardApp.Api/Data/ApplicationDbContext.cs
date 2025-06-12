namespace FlashcardApp.Api.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> dbContextOptions) : base(dbContextOptions)
        {

        }

        public DbSet<Deck> Decks { get; set; }
        public DbSet<Card> Cards { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Settings> Settings { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            //optionsBuilder.LogTo(Console.WriteLine, LogLevel.Information)
            //    .EnableDetailedErrors();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // ApplicationUser to Deck (One-to-Many)
            builder.Entity<ApplicationUser>()
                .HasMany(u => u.Decks)
                .WithOne(d => d.User)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // ApplicationUser to Settings (One-to-Many)
            builder.Entity<ApplicationUser>()
                .HasMany(u => u.Settings)
                .WithOne(s => s.User)
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Deck to Card (One-to-Many)
            builder.Entity<Deck>()
                .HasMany(d => d.Cards)
                .WithOne(c => c.Deck)
                .HasForeignKey(c => c.DeckId)
                .OnDelete(DeleteBehavior.Cascade);

            // Card to Review (One-to-Many)
            builder.Entity<Card>()
                .HasMany(c => c.Reviews)
                .WithOne(r => r.Card)
                .HasForeignKey(r => r.CardId)
                .OnDelete(DeleteBehavior.Cascade);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            OnBeforeSaving();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        private void OnBeforeSaving()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is Deck || e.Entity is Card || e.Entity is Review || e.Entity is Settings || e.Entity is ApplicationUser);

            foreach (var entry in entries)
            {
                var now = DateTime.UtcNow;
                switch (entry.State)
                {
                    case EntityState.Added:
                        if (entry.Metadata.FindProperty("CreatedAt") != null)
                        {
                            entry.Property("CreatedAt").CurrentValue = now;
                        }

                        if (entry.Metadata.FindProperty("UpdatedAt") != null)
                        {
                            entry.Property("UpdatedAt").CurrentValue = now;
                        }
                        break;

                    case EntityState.Modified:
                        if (entry.Metadata.FindProperty("UpdatedAt") != null)
                        {
                            entry.Property("UpdatedAt").CurrentValue = now;
                        }

                        if (entry.Metadata.FindProperty("CreatedAt") != null)
                        {
                            entry.Property("CreatedAt").IsModified = false;
                        }
                        break;
                }
            }
        }
    }
}