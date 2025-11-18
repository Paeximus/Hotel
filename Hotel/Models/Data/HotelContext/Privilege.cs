using System;
using System.Collections.Generic;

namespace Hotel.Models.Data.HotelContext;

public partial class Privilege
{
    public int PrivilegeId { get; set; }

    public string Description { get; set; } = null!;

    public int RoomId { get; set; }

    public virtual Room Room { get; set; } = null!;
}
