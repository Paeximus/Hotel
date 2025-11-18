using System;
using System.Collections.Generic;

namespace Hotel.Models.Data.HotelContext;

public partial class Role
{
    public required string RoleId { get; set; }

    public string? RoleName { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
