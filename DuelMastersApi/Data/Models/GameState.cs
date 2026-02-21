using System;

namespace DuelMastersApi.Data.Models
{
    public class GameState
    {
        public int Id { get; set; }
        public int MatchId { get; set; }
        public Match? Match { get; set; }
        public int Turn { get; set; }
        public string State { get; set; } = "{}"; // JSON payload
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
