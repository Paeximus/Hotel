using Hotel.Models.Data.HotelContext;
using Hotel.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace Hotel.Services
{
    public class RoomServices
    {
        private readonly HotelContext _context;
        private readonly ILogger<RoomServices> _logger;
        public RoomServices(HotelContext context, ILogger<RoomServices> logger)
        {
            _logger = logger;
            _context = context;
        }
        public List<RoomViewModel> GetUnreservedRooms()
        {
            List<Reservation> reservation = _context.Reservations.Include(x => x.Room).Distinct().ToList();
            List<int> model1 = reservation.Select(r => r.RoomId).ToList();
            List<RoomViewModel> unreservedrooms = GetRooms(model1);

            return unreservedrooms;
        }

       

        public  List<RoomViewModel> GetRooms(List<int> mod)
        {
            
            List<Room> rooms = _context.Rooms.Include(x => x.Floor)
                                                     .Include(x => x.Privileges)
                                                     .Where(x=>!mod.Contains(x.RoomId))
                                                     .ToList();
  
            List<RoomViewModel> model = rooms.Select(p => new RoomViewModel
            {
                RoomId = p.RoomId,
                RoomNo = p.RoomNo,
                MaxOccupants = p.MaxOccupants,
                RoomImage = p.RoomImage,
                Price = p.Price,
                FloorId = p.FloorId,
                Floor = p.Floor
            }).ToList();
            return model;
        }

        public List<RoomViewModel> GetRooms()
        {

            List<Room> rooms = _context.Rooms.Include(x => x.Floor)
                                                     .Include(x => x.Privileges)
                                                     .ToList();
            List<Reservation> resevations = _context.Reservations.Include(r => r.Room).ToList();
            List<RoomViewModel> model = rooms.Select(p => new RoomViewModel
            {
                RoomId = p.RoomId,
                RoomNo = p.RoomNo,
                MaxOccupants = p.MaxOccupants,
                RoomImage = p.RoomImage,
                Price = p.Price,
                IsOccupied= p.IsOccupied,
                FloorId = p.FloorId,
                Floor = p.Floor
            }).ToList();
            return model;
        }

        public RoomViewModel GetRoom(int roomId)
        {
            Room room = _context.Rooms.Where(p => p.RoomId == roomId).Include(p => p.Floor)
                                                                                    .Include(p => p.Privileges)
                                                                                    .FirstOrDefault();
            if (room == null) return new RoomViewModel();
            RoomViewModel model = new RoomViewModel
            {
                RoomId = room.RoomId,
                RoomNo = room.RoomNo,
                MaxOccupants = room.MaxOccupants,
                IsOccupied = room.IsOccupied,
                RoomImage = room.RoomImage,
                Price = room.Price,
                FloorId = room.FloorId,
                Floor=room.Floor,
            };
            return model;
        }

        public IEnumerable<Room> GetAvailableRooms(DateOnly checkIn, DateOnly checkOut)
        {
            try
            {
                var bookedRoomIds = _context.Reservations
                    .Where(b => (b.ArrivalDate < checkOut) && (b.DateOfExit > checkIn))
                    .Select(b => b.RoomId)
                    .Distinct()
                    .ToList();
                var availableRooms = _context.Rooms
                    .Where(r => !bookedRoomIds.Contains(r.RoomId))
                    .ToList();
                _logger.LogInformation("Found {Count} available rooms between {CheckIn} and {CheckOut}", availableRooms.Count, checkIn, checkOut);
                return availableRooms;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetAvailableRooms failed for dates {CheckIn} - {CheckOut}", checkIn, checkOut);
                return Enumerable.Empty<Room>();
            }
        }

       
        public bool AddRoom(RoomViewModel model) 
        {
            try
            {
                Room room = new Room();
                using (var memoryStream = new MemoryStream())
                {
                    model.RoomImageFile?.CopyTo(memoryStream);
                    model.RoomImage = memoryStream.ToArray();
                }
                room.RoomNo = model.RoomNo;
                room.MaxOccupants = model.MaxOccupants;
                room.IsOccupied = "N";
                room.Price = model.Price;
                room.FloorId = model.FloorId;
                room.RoomImage = model.RoomImage;
                    
                _context.Rooms.Add(room);
                _context.SaveChanges();
                _logger.LogInformation("Added new room with ID {RoomId}", room.RoomId);
                return true;
            }
            
            catch (Exception ex)
            {
                _logger.LogError(ex, "AddRoom failed for room number {RoomNo}", model.RoomNo);
                return false;
            }
        }

        public bool EditRoom(RoomViewModel model)
        {
            try
            {
                Room? room = _context.Rooms.Where(x => x.RoomId == model.RoomId).FirstOrDefault();


                if (model.RoomImageFile != null && model.RoomImageFile.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        model.RoomImageFile?.CopyTo(memoryStream);
                        model.RoomImage = memoryStream.ToArray();
                    }
                    room.RoomImage = model.RoomImage;
                }
                room.RoomNo = model.RoomNo;
                room.Price = model.Price;
                room.MaxOccupants = model.MaxOccupants;
                room.IsOccupied = model.IsOccupied;
                room.FloorId = model.FloorId;
                _context.Rooms.Update(room);
                _context.SaveChanges();
                _logger.LogInformation("Added new room with ID {RoomId}", room.RoomId);

                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }

        public bool DeleteRoom(int roomId)
        {
            try
            {
                Room? room = _context.Rooms.Where(x => x.RoomId == roomId).FirstOrDefault();
                if (room == null) return false;
                _context.Rooms.Remove(room);
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

