namespace Hotel.Models.ViewModels
{
    public class MultiViewModel
    {
        public ReservationViewModel ReservationViewModel { get; set; } = new ReservationViewModel();
        public RoomViewModel RoomViewModel { get; set; } = new RoomViewModel();
    }
}
