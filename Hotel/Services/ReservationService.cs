using Hotel.Models.Data.HotelContext;
using Hotel.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace Hotel.Services
{
    [Authorize]
    public class ReservationService
    {
        private readonly HotelContext _context;
        private readonly ILogger<ReservationService> _logger;

        public ReservationService(HotelContext context, ILogger<ReservationService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IEnumerable<ReservationViewModel> SeeReservation(int userid)
        {
            IEnumerable<Reservation> reservation = _context.Reservations.Where(r => r.UserId == userid).Include(x=>x.Room).Include(x=>x.User).ToList();

            
            IEnumerable<ReservationViewModel> model = reservation.Select(p => new ReservationViewModel
            {
                ReservationId = p.ReservationId,
                RoomId = p.RoomId,
                ReservationDate = p.ReservationDate,
                ArrivalDate = p.ArrivalDate,
                DateOfExit = p.DateOfExit,
                WithCar = p.WithCar,
                ModeOfOrder = p.ModeOfOrder,
                CarRegNo = p.CarRegNo,
                UserId = p.UserId,
                Room = p.Room,
                User = p.User,
            }).ToList();

            return model;

        }

        public ReservationViewModel GetReservation(int Reservationid)
        {
            Reservation reservation = _context.Reservations.Where(r => r.ReservationId == Reservationid)
                .Include(r => r.Room)
                .Include(r => r.User)
                .FirstOrDefault();


            if (reservation == null) return null;
            ReservationViewModel model = new ReservationViewModel
            {
                ReservationId = reservation.ReservationId,
                RoomId = reservation.RoomId,
                ReservationDate = reservation.ReservationDate,
                ArrivalDate = reservation.ArrivalDate,
                DateOfExit = reservation.DateOfExit,
                WithCar = reservation.WithCar,
                ModeOfOrder = reservation.ModeOfOrder,
                CarRegNo = reservation.CarRegNo,
                UserId = reservation.UserId,
                Room = reservation.Room,
                User = reservation.User,
            };

            return model;

        }
        public IEnumerable<ReservationViewModel> GetAllResevations()
        {

            IEnumerable<Reservation> reservation = _context.Reservations.Include(x => x.Room)
                                                     .Include(x => x.User)
                                                     .ToList();
            IEnumerable<ReservationViewModel> model = reservation.Select(p => new ReservationViewModel
            {
                ReservationId = p.ReservationId,
                RoomId = p.RoomId,
                ReservationDate = p.ReservationDate,
                ArrivalDate = p.ArrivalDate,
                DateOfExit = p.DateOfExit,
                WithCar = p.WithCar,
                ModeOfOrder = p.ModeOfOrder,
                CarRegNo = p.CarRegNo,
                UserId = p.UserId,
                Room = p.Room,
                User = p.User,

            }).ToList();
            
            return model;
        }

        public bool AddReservation(ReservationViewModel model, string userId, string roomId)
        {
            if (userId != null)
                Console.WriteLine("UserId exists");
            else
                Console.WriteLine("UserId is null");
            _logger.LogWarning("UserId is null");
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

        public bool EditReservation(ReservationViewModel model)
        {
            try
            {
                Reservation? reservation = _context.Reservations.Where(x => x.ReservationId == model.ReservationId).FirstOrDefault();

                reservation.RoomId = model.RoomId;
                reservation.ReservationDate = model.ReservationDate;
                reservation.ArrivalDate = model.ArrivalDate;
                reservation.DateOfExit = model.DateOfExit;
                reservation.WithCar = model.WithCar;
                reservation.ModeOfOrder = model.ModeOfOrder;
                reservation.CarRegNo = model.CarRegNo;
                reservation.UserId = model.UserId;
                reservation.Room = model.Room;
                reservation.User = model.User;
                _context.Reservations.Update(reservation);
                _context.SaveChanges();
                _logger.LogInformation("Added new room with ID {RoomId}", reservation.RoomId);

                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }

        public bool DeleteReservation(int reservationId)
        {
            try
            {
                Reservation? reservation = _context.Reservations.Where(r => r.ReservationId == reservationId).FirstOrDefault();
                if (reservation == null) return false;
                _context.Reservations.Remove(reservation);
                _context.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}