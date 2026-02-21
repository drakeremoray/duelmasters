using System;

namespace DuelMastersApi.Data.Models
{
    public class MatchAction
    {
        public int Id { get; set; }
        public int MatchId { get; set; }
        public int PlayerId { get; set; }
        public string ActionType { get; set; } = string.Empty;
        public string Payload { get; set; } = "{}"; // JSON
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
