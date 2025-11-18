namespace Hotel.Models.ViewModels
{
    public class ReservationViewModel
    {
        public int ReservationId { get; set; }

        public DateOnly ArrivalDate { get; set; }

        public DateOnly DateOfExit { get; set; }

        public string ModeOfOrder { get; set; } = null!;

        public string WithCar { get; set; } = null!;

        public string? CarRegNo { get; set; }

        public int UserId { get; set; }

        public int RoomId { get; set; }
    }
}
