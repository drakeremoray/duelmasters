using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DuelMastersApi.Data.Models
{
    public class Player
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string Username { get; set; } = default!;

        [JsonIgnore]
        public string? PasswordHash { get; set; }

        public string? DisplayName { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
