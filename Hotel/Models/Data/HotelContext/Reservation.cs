using System;
using System.Collections.Generic;

namespace Hotel.Models.Data.HotelContext;

public partial class Reservation
{
    public int ReservationId { get; set; }

    public DateTime ReservationDate { get; set; }

    public DateOnly ArrivalDate { get; set; }

    public DateOnly DateOfExit { get; set; }

    public string ModeOfOrder { get; set; } = null!;

    public string WithCar { get; set; } = null!;

    public string? CarRegNo { get; set; }

    public int UserId { get; set; }

    public int RoomId { get; set; }

    public virtual Room Room { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
