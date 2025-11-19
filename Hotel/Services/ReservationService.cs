using Hotel.Models.Data.HotelContext;
using Hotel.Models.ViewModels;

namespace Hotel.Services
{
    public class ReservationService
    {
        private readonly HotelContext _context;
        private readonly ILogger<ReservationService> _logger;

        public ReservationService(HotelContext context, ILogger<ReservationService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IEnumerable<Reservation> GetAllResevations()
        {
            return _context.Reservations.ToList();
        }
        public bool AddReservation(ReservationViewModel model, string userId, string roomId)
        {
            if (userId != null)
                Console.WriteLine("UserId exists");
            else
                Console.WriteLine("UserId is null");
            try
            {
                Reservation reservation = new Reservation
                {
                    ReservationId = model.ReservationId,
                    ReservationDate = DateTime.Now,
                    ArrivalDate = model.ArrivalDate,
                    DateOfExit = model.DateOfExit,
                    ModeOfOrder = model.ModeOfOrder,
                    WithCar = model.WithCar,  
                    CarRegNo = model.CarRegNo,
                    //RoomId = model.RoomId
                };

                var userIdProp = typeof(Reservation).GetProperty("UserId");
                if (userIdProp != null)
                {
                    var propType = userIdProp.PropertyType;
                    if (propType == typeof(string))
                    {
                        userIdProp.SetValue(reservation, userId);
                    }
                    else if (propType == typeof(int) && int.TryParse(userId, out var uidInt))
                    {
                        userIdProp.SetValue(reservation, uidInt);
                    }
                    else if (propType == typeof(Guid) && Guid.TryParse(userId, out var uidGuid))
                    {
                        userIdProp.SetValue(reservation, uidGuid);
                    }
                    else
                    {
                        _logger.LogWarning("Reservation.UserId property type {Type} not handled; value not set", propType);
                    }
                }
                var userIdProp1 = typeof(Reservation).GetProperty("RoomId");
                if (userIdProp1 != null)
                {
                    var propType = userIdProp1.PropertyType;
                    if (propType == typeof(string))
                    {
                        userIdProp1.SetValue(reservation, roomId);
                    }
                    else if (propType == typeof(int) && int.TryParse(roomId, out var uidInt))
                    {
                        userIdProp1.SetValue(reservation, uidInt);
                    }
                    else if (propType == typeof(Guid) && Guid.TryParse(roomId, out var uidGuid))
                    {
                        userIdProp1.SetValue(reservation, uidGuid);
                    }
                    else
                    {
                        _logger.LogWarning("Reservation.UserId property type {Type} not handled; value not set", propType);
                    }
                }
                _context.Reservations.Add(reservation);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding resevation");
                return false;
            }
        }
    }
}