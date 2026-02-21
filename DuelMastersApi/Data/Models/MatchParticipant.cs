using System;

namespace DuelMastersApi.Data.Models
{
    public class MatchParticipant
    {
        public int Id { get; set; }
        public int MatchId { get; set; }
        public int PlayerId { get; set; }
        public bool IsSpectator { get; set; } = false;
        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    }
}
