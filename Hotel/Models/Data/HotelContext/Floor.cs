using System;
using System.Collections.Generic;

namespace Hotel.Models.Data.HotelContext;

public partial class Floor
{
    public int FloorId { get; set; }

    public int FloorNo { get; set; }

    public string FloorName { get; set; } = null!;
    public virtual ICollection<Room> Rooms { get; set; } = new List<Room>();
}
