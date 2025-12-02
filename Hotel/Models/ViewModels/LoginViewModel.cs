using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hotel.Models.ViewModels
{
    public class LoginViewModel
    {
        
        [Required]
        public string Email { get; set; } = null!;
        [Required]
        public string Password { get; set; } = null!;

        [NotMapped]
        public int UserId { get; set; }
    }
}
