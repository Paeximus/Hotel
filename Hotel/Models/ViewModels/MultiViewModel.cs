using Hotel.Models.Data.HotelContext;
using Hotel.Models.ViewModels;

namespace Hotel.Models.ViewModels
{
    public class MultiViewModel
    {
        public ReservationViewModel ReservationViewModel { get; set; } = new ReservationViewModel();
        public Reservation Reservation { get; set; } = new Reservation();
    }
}
