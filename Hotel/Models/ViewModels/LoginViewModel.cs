using System.ComponentModel.DataAnnotations;

namespace Hotel.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        public string Email { get; set; } = null!;
        [Required]
        public string Password { get; set; } = null!;
    }
}
