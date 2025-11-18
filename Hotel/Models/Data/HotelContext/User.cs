using System;
using System.Collections.Generic;

namespace Hotel.Models.Data.HotelContext;

public partial class User
{
    public int UserId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string? OtherNames { get; set; }

    public string? Gender { get; set; }

    public DateOnly? DateOfBirth { get; set; }

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public required string RoleId { get; set; }

    public virtual ICollection<Complaint> Complaints { get; set; } = new List<Complaint>();

    public virtual ICollection<Reservation> Resevations { get; set; } = new List<Reservation>();

    public virtual Role Role { get; set; } = null!;
}
