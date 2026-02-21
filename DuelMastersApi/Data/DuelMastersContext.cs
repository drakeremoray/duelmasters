using Microsoft.EntityFrameworkCore;
using DuelMastersApi.Data.Models;

namespace DuelMastersApi.Data
{
    public class DuelMastersContext : DbContext
    {
        public DuelMastersContext(DbContextOptions<DuelMastersContext> options) : base(options) { }

        public DbSet<Player> Players => Set<Player>();
        public DbSet<Card> Cards => Set<Card>();
        public DbSet<Deck> Decks => Set<Deck>();
        public DbSet<DeckCard> DeckCards => Set<DeckCard>();
        public DbSet<Match> Matches => Set<Match>();
        public DbSet<GameState> GameStates => Set<GameState>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Player>().HasIndex(p => p.Username).IsUnique();
            modelBuilder.Entity<GameState>().Property(g => g.State).HasColumnType("jsonb");
            modelBuilder.Entity<DeckCard>().Property(dc => dc.Quantity).HasDefaultValue(1);
        }
    }
}
