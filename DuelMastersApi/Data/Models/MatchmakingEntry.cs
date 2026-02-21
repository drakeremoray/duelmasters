using System;

namespace DuelMastersApi.Data.Models
{
    public class MatchmakingEntry
    {
        public int Id { get; set; }
        public int PlayerId { get; set; }
        public int Skill { get; set; } = 1000;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        // Pending | Matched
        public string Status { get; set; } = "Pending";
        public int? MatchId { get; set; }
    }
}
