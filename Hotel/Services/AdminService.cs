using Hotel.Models.Data.HotelContext;
using Hotel.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Hotel.Services
{
    public class AdminService
    {
        private readonly HotelContext _context;
        private readonly PasswordHasher<User> _hasher;
        private readonly ILogger<AdminService> _logger;

        // Updated constructor - don't inject PasswordHasher
        public AdminService(HotelContext context, ILogger<AdminService> logger)
        {
            _context = context;
            _logger = logger;
            _hasher = new PasswordHasher<User>(); 
        }

        public IEnumerable<AccountViewModel> GetAllClients()
        {
            try
            {
                List<User> users = _context.Users
                    .Include(x => x.Resevations)
                    .Include(x => x.Role)
                    .ToList();

                IEnumerable<AccountViewModel> model = users.Select(p => new AccountViewModel
                {
                    UserId = p.UserId,
                    FirstName = p.FirstName,
                    LastName = p.LastName,
                    OtherNames = p.OtherNames,
                    Gender = p.Gender,
                    DateOfBirth = p.DateOfBirth,
                    Email = p.Email,
                    Role = p.Role,
                    RoleId = p.RoleId
                }).ToList();

                return model;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all clients");
                throw;
            }
        }

        public DashboardViewModel GetDashboardData(string userName)
        {
            try
            {
                // Group reservations by room type
                var roomTypeData = _context.Reservations
                    .Include(r => r.Room)
                    .GroupBy(r => r.Room.MaxOccupants)
                    .Select(g => new { RoomType = g.Key, Count = g.Count() })
                    .ToList();

                // Group reservations by day of week
                var dayData = _context.Reservations
                    .AsEnumerable()
                    .GroupBy(r => r.ArrivalDate.DayOfWeek)
                    .Select(g => new
                    {
                        Day = g.Key,
                        DayName = g.Key.ToString().Substring(0, 3),
                        Count = g.Count()
                    })
                    .OrderBy(x => (int)x.Day)
                    .ToList();

                // Ensure we have data for all days of the week
                var allDays = Enumerable.Range(0, 7)
                    .Select(i => (DayOfWeek)i)
                    .Select(day => new
                    {
                        Day = day,
                        DayName = day.ToString().Substring(0, 3),
                        Count = dayData.FirstOrDefault(d => d.Day == day)?.Count ?? 0
                    })
                    .ToList();

                var model = new DashboardViewModel
                {
                    UserName = userName,
                    TotalReservations = _context.Reservations.Count(),
                    TotalUsers = _context.Users.Count(),
                    TotalRooms = _context.Rooms.Count(),
                    RoomTypes = roomTypeData.Select(x => x.RoomType switch
                    {
                        1 => "Single",
                        2 => "Double",
                        3 => "Group",
                        4 => "Family",
                        _ => $"Room for {x.RoomType}"
                    }).ToList(),
                    RoomTypeCounts = roomTypeData.Select(x => x.Count).ToList(),
                    DaysOfWeek = allDays.Select(x => x.DayName).ToList(),
                    ReservationsPerDay = allDays.Select(x => x.Count).ToList()
                };

                _logger.LogInformation($"Dashboard data loaded successfully for {userName}");
                return model;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading dashboard data");
                throw;
            }
        }
    }
}