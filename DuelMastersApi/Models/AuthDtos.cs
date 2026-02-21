using System.ComponentModel.DataAnnotations;

namespace DuelMastersApi.Models
{
    public class RegisterDto
    {
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string Username { get; set; } = default!;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = default!;

        public string? DisplayName { get; set; }
    }

    public class LoginDto
    {
        [Required]
        public string Username { get; set; } = default!;

        [Required]
        public string Password { get; set; } = default!;
    }
}
