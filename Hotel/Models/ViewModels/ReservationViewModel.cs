using Hotel.Models.Data.HotelContext;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace Hotel.Models.ViewModels
{
    public class ReservationViewModel
    {
        public DateTime ReservationDate { get; set; }

        public int ReservationId { get; set; }

        public DateOnly ArrivalDate { get; set; }

        public DateOnly DateOfExit { get; set; }

        public string ModeOfOrder { get; set; } = null!;

        public string WithCar { get; set; } = null!;

        public string? CarRegNo { get; set; }

        public int UserId { get; set; }

        public int RoomId { get; set; }

        [NotMapped]
        [ValidateNever]
        public  byte[] RoomImage { get; set; } = Array.Empty<byte>();

        [NotMapped]
        [ValidateNever]
        public virtual Room Room { get; set; } = null!;

        [NotMapped]
        [ValidateNever]
        public virtual User User { get; set; } = null!;

        [NotMapped]
        [ValidateNever]
        public virtual TimeOnly ReservationTimeOnly { get; set; }

        [NotMapped]
        [ValidateNever]
        public virtual DateOnly ReservationDateOnly { get; set; }

    }
}
