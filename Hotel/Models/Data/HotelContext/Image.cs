using System;
using System.Collections.Generic;

namespace Hotel.Models.Data.HotelContext;

public partial class Image
{
    public int ImageId { get; set; }

    public byte[] Image1 { get; set; } = null!;

    public int RoomId { get; set; }

    public virtual Room Room { get; set; } = null!;
}
