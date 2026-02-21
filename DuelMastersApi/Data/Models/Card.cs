using System;

namespace DuelMastersApi.Data.Models
{
    public class Card
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public string? CardType { get; set; }
        public int Cost { get; set; }
        public string? Text { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
