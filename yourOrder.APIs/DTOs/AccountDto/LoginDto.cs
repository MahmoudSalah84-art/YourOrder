using System.ComponentModel.DataAnnotations;

namespace yourOrder.APIs.DTOs.Account
{
    public class LoginDto
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
