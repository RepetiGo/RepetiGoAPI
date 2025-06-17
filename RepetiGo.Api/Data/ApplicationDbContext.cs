
namespace RepetiGo.Api.Data
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
            ConfigureTimeStamps(builder);

            // ApplicationUser to Deck (One-to-Many)
            builder.Entity<ApplicationUser>()
                .HasMany(u => u.Decks)
                .WithOne(d => d.User)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // ApplicationUser to Settings (One-to-One)
            builder.Entity<ApplicationUser>()
                .HasOne(u => u.Settings)
                .WithOne(s => s.User)
                .HasForeignKey<Settings>(s => s.UserId)
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

        private void ConfigureTimeStamps(ModelBuilder builder)
        {
            var entities = new Type[]
            {
                typeof(Deck),
                typeof(Card),
                typeof(Review),
                typeof(Settings),
                typeof(ApplicationUser)
            };

            foreach (var entityType in entities)
            {
                var entity = builder.Entity(entityType);

                if (entity.Metadata.FindProperty("CreatedAt") is not null)
                {
                    entity.Property<DateTime>("CreatedAt")
                        .HasDefaultValueSql("GETUTCDATE()") // Only works on INSERT
                        .ValueGeneratedOnAdd(); // Only works on INSERT
                }
            }
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.State == EntityState.Modified && entry.Properties.Any(p => p.Metadata.Name == "UpdatedAt"))
                {
                    entry.Property("UpdatedAt").CurrentValue = DateTime.UtcNow;
                }
            }
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }
    }
}