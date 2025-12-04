using Hotel.Models.Data.HotelContext;
using Hotel.Models.ViewModels;

namespace Hotel.Models.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalRooms { get; set; }
        public int TotalReservations { get; set; }
        public int TotalUsers { get; set; }
        public string UserName { get; set; }
        public List<string> RoomTypes { get; set; }
        public List<int> RoomTypeCounts { get; set; }

        public List<string> DaysOfWeek { get; set; }
        public List<int> ReservationsPerDay { get; set; }
    }
}
