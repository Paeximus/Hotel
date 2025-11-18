using Hotel.Models.Data.HotelContext;

namespace Hotel.Models.ViewModels
{
    public class RoleViewModel
    {
        public required string RoleId { get; set; }

        public string? RoleName { get; set; }

        public virtual ICollection<User> Users { get; set; } = new List<User>();
    }
}
