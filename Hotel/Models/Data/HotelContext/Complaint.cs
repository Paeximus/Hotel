using System;
using System.Collections.Generic;

namespace Hotel.Models.Data.HotelContext;

public partial class Complaint
{
    public int ConplaintId { get; set; }

    public string? Description { get; set; }

    public DateTime DateOfComplaint { get; set; }

    public int RoomId { get; set; }

    public int UserId { get; set; }

    public virtual Room Room { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
