using System;

namespace DuelMastersApi.Data.Models
{
    public class Deck
    {
        public int Id { get; set; }
        public int PlayerId { get; set; }
        public Player? Player { get; set; }
        public string Name { get; set; } = default!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
