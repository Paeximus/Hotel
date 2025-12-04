using Hotel.Filters;
using Hotel.Models.Data.HotelContext;
using Hotel.Models.ViewModels;
using Hotel.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Hotel.Controllers
{
    [AuthorizeRole("1")]
    public class AdminController : Controller
    {
        private readonly AdminService _adminService;
        private readonly HotelContext _context;
        public AdminController(HotelContext context, AdminService adminService)
        {
            _context = context;
            _adminService = adminService;
        }
        // GET: AdminController
        public ActionResult Index()
        {
            var model = _adminService.GetAllClients();
            return View(model);
        }

        public ActionResult Dashboard()
        {
            var fullname = "";
            if (User == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                if (User?.Identity?.IsAuthenticated == true)
                {
                    var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    if (int.TryParse(userIdClaim, out var userId))
                    {
                        var user = _context.Users
                            .AsNoTracking()
                            .FirstOrDefault(a => a.UserId == userId);
                        if (user != null)
                        {
                            userId = user.UserId;
                            fullname = user.FirstName + " " + user.LastName + " " + user.OtherNames;
                        }
                    }
                }
            }
            
            var userName = User.Identity?.Name ?? "Guest";

            // Group reservations by room type
            var roomTypeData = _context.Reservations
                .GroupBy(r => r.Room.MaxOccupants)
                .Select(g => new { RoomType = g.Key, Count = g.Count() })
                .ToList();

            // Group reservations by day of week (properly ordered)
            var dayData = _context.Reservations
                .AsEnumerable()
                .GroupBy(r => r.ArrivalDate.DayOfWeek)
                .Select(g => new {
                    Day = g.Key,
                    DayName = g.Key.ToString(),
                    Count = g.Count()
                })
                .OrderBy(x => (int)x.Day) // Order by day number (0=Sunday, 1=Monday, etc.)
                .ToList();

            // Ensure we have data for all days of the week (fill missing days with 0)
            var allDays = Enumerable.Range(0, 7)
                .Select(i => (DayOfWeek)i)
                .Select(day => new
                {
                    Day = day,
                    DayName = day.ToString().Substring(0, 3), // Mon, Tue, Wed, etc.
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

            return View(model);
        }

        // GET: AdminController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AdminController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: AdminController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: AdminController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: AdminController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: AdminController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
