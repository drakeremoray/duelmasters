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
        public DbSet<MatchAction> MatchActions => Set<MatchAction>();
        public DbSet<MatchmakingEntry> MatchmakingQueue => Set<MatchmakingEntry>();
        public DbSet<MatchParticipant> MatchParticipants => Set<MatchParticipant>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Player>().HasIndex(p => p.Username).IsUnique();
            modelBuilder.Entity<GameState>().Property(g => g.State).HasColumnType("jsonb");
            modelBuilder.Entity<DeckCard>().Property(dc => dc.Quantity).HasDefaultValue(1);
            modelBuilder.Entity<MatchAction>().Property(a => a.Payload).HasColumnType("jsonb");
            modelBuilder.Entity<MatchmakingEntry>().HasIndex(m => m.CreatedAt);
            modelBuilder.Entity<MatchmakingEntry>().HasIndex(m => m.PlayerId);
            modelBuilder.Entity<MatchParticipant>().HasIndex(p => new { p.MatchId, p.PlayerId });
        }
    }
}
