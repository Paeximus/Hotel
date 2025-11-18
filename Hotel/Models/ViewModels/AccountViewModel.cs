using Hotel.Models.Data.HotelContext;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Hotel.Models.ViewModels
{
    public class AccountViewModel
    {
        [Key]
        public int UserId { get; set; }

        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;
        [AllowNull]
        public string? OtherNames { get; set; } 

        public string? Gender { get; set; }

        public DateOnly? DateOfBirth { get; set; }

        public string Email { get; set; } = null!;

        public string Password { get; set; } = null!;

        public required string RoleId { get; set; } 

    }
}
