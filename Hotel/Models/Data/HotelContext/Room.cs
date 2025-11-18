using System;
using System.Collections.Generic;

namespace Hotel.Models.Data.HotelContext;

public partial class Room
{
    public int RoomId { get; set; }

    public string RoomNo { get; set; } = null!;

    public int MaxOccupants { get; set; }

    public string IsOccupied { get; set; } = null!;

    public decimal Price { get; set; }

    public int FloorId { get; set; }

    public byte[]? RoomImage { get; set; } = null!;

    public virtual ICollection<Complaint> Complaints { get; set; } = new List<Complaint>();

    public virtual Floor? Floor { get; set; }

    public virtual ICollection<Image> Images { get; set; } = new List<Image>();

    public virtual ICollection<Privilege> Privileges { get; set; } = new List<Privilege>();

    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
