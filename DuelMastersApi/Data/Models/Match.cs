using System;

namespace DuelMastersApi.Data.Models
{
    public class Match
    {
        public int Id { get; set; }
        public DateTime StartedAt { get; set; } = DateTime.UtcNow;
        public DateTime? FinishedAt { get; set; }
        public string? Metadata { get; set; }
    }
}
