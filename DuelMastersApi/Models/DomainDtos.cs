using System.ComponentModel.DataAnnotations;

namespace DuelMastersApi.Models
{
    public class CardDto
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = default!;

        public string? CardType { get; set; }

        [Range(0, 100)]
        public int Cost { get; set; }

        public string? Text { get; set; }
    }

    public class DeckDto
    {
        [Required]
        public int PlayerId { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; } = default!;
    }

    public class DeckCardDto
    {
        [Required]
        public int DeckId { get; set; }

        [Required]
        public int CardId { get; set; }

        [Range(1, 99)]
        public int Quantity { get; set; } = 1;
    }

    public class MatchDto
    {
        public string? Metadata { get; set; }
    }

    public class GameStateDto
    {
        [Required]
        public int MatchId { get; set; }

        [Range(0, int.MaxValue)]
        public int Turn { get; set; }

        [Required]
        public string State { get; set; } = "{}"; // JSON payload
    }
}
