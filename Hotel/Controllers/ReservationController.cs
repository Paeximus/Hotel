using Hotel.Models.Data.HotelContext;
using Hotel.Models.ViewModels;
using Hotel.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.UserSecrets;
using System.Security.Claims;


namespace Hotel.Controllers
{
    [Authorize]
    public class ReservationController : Controller
    {
        private readonly HotelContext _context;
        private readonly ReservationService _reservationService;
        private readonly ILogger<ReservationController> _logger;

        public ReservationController(ReservationService reservationService, ILogger<ReservationController> logger, HotelContext context)
        {
            _reservationService = reservationService;
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            var userid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            _logger.LogInformation("Accessed Reservation Index");

            if (User.IsInRole("Admin"))
            {
                var reservations = _reservationService.GetAllResevations();
                reservations = reservations.OrderBy(r => r.ReservationDate).ToList();
                return View(reservations);
            }
            else
            {
                var reservation = _reservationService.SeeReservation(int.Parse(userid));
                return View(reservation);
            }
        }

        // GET: Reservation/Add?roomId=123
        public IActionResult Add(int? roomId)
        {

            ReservationViewModel model = new ReservationViewModel();
            if (roomId.HasValue)
            {
                model.RoomId = roomId.Value;
                ViewBag.SelectedRoomId = roomId.Value;
            }
            else
            {
                ViewBag.SelectedRoomId = null;
            }

            return View(model);
        }

        // POST: Reservation/Add
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Add(ReservationViewModel model, int? roomId)
        {
            var userid = User.FindFirstValue(ClaimTypes.NameIdentifier) ;
            model.UserId = int.Parse(userid);
            if (model.UserId == 0)
            {
                ModelState.AddModelError("", "User Id is null");
                Console.WriteLine("User Id is null");
            }
            if (model.RoomId == 0)
            {
                ModelState.AddModelError("", "roomId is null");
                Console.WriteLine("Room Id is null");
            }
            if (model.ModeOfOrder == null)
            {
                ModelState.AddModelError("", "Mode of Error is null");
                Console.WriteLine("Mode of Error is null");
            }
            
            if (!ModelState.IsValid)
            {
                Console.WriteLine("Model state is invalid");
                ViewBag.SelectedRoomId = roomId ?? (object?)null;
                return View(model);
            }

            if (roomId.HasValue)
            {
                model.RoomId = roomId.Value;
                Console.WriteLine($"Using roomId: {model.RoomId}");
            }
            else
                Console.WriteLine("No roomId provided in query or route; using model.RoomId from form data.");

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("Attempt to add reservation without user id");
                return Challenge();
            }

            
            string modeOfOrder;
            var roleClaim = User.FindFirst("RoleId") ?? User.FindFirst(ClaimTypes.Role);
            if (roleClaim != null)
            {
                if (int.TryParse(roleClaim.Value, out var roleIdNumeric))
                {
                    modeOfOrder = roleIdNumeric == 1 ? "Desk" : "Online";
                }
                else
                {
                    // handle role name (e.g., "Admin" -> desk)
                    var roleName = roleClaim.Value;
                    modeOfOrder = string.Equals(roleName, "Admin", StringComparison.OrdinalIgnoreCase) ||
                                  string.Equals(roleName, "Client", StringComparison.OrdinalIgnoreCase) ||
                                  roleName == "1"
                                  ? "Desk" : "Online";
                }
            }
            else
            {
                // No role claim - default to Online
                modeOfOrder = "Online";
            }
            model.ModeOfOrder = modeOfOrder;
            var roomId1 = roomId.ToString();
            var created = _reservationService.AddReservation(model, userId, roomId1);
            if (model.ArrivalDate>model.DateOfExit)
            {
                ModelState.AddModelError("DateOfExit", "Exit Date cannot be before Arrival Date");
                return View(model);
            }

            return RedirectToAction("Index", "Reservation");
        }

        public ActionResult Edit(int id)
        {
            var model = _context.Reservations
                .AsNoTracking()
                .Include(r => r.Room)
                .Include(r=> r.User)
                .FirstOrDefault(r => r.ReservationId == id);

            if (model == null)
            {
                _logger.LogWarning("Edit called for non-existent reservation id {ReservationId}", id);
                return NotFound();
            }

            var reservation = new ReservationViewModel
            {
                ReservationId = model.ReservationId,
                RoomId = model.RoomId,
                ReservationDate = model.ReservationDate,
                ArrivalDate = model.ArrivalDate,
                DateOfExit = model.DateOfExit,
                WithCar = model.WithCar,
                ModeOfOrder = model.ModeOfOrder,
                CarRegNo = model.CarRegNo,
                UserId = model.UserId,
                Room = model.Room,
                User = model.User,
            };

            return View(reservation);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ReservationViewModel model)
        {
            try
            {
                var userid = User.FindFirstValue(ClaimTypes.NameIdentifier);
                model.UserId = int.Parse(userid);
                if (model.RoomId == 0)
                {
                    _logger.LogWarning("RoomId is 0 in POST Edit");
                    ModelState.AddModelError("", "Invalid Room ID");
                    return View(model);
                }
                if (ModelState.IsValid)
                {
                    bool result = _reservationService.EditReservation(model);
                    if (result)
                    {
                        return RedirectToAction(nameof(Index));
                    }

                    ModelState.AddModelError(string.Empty, "Could not save changes. Try again.");
                }
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Edit failed for room id {model.ReservationId}", model);
                ModelState.AddModelError("Error", "Ooops! Something went wrong!");
                return View(model);
            }
        }

        public ActionResult Delete(int id)
        {
            var reservation = _reservationService.GetReservation(id);

            if (reservation == null)
            {
                _logger.LogWarning("Delete called for non-existent reservation id {ReservationId}", id);
                TempData["ErrorMessage"] = "Reservation not found.";
                return RedirectToAction(nameof(Index));
            }

            return View(reservation);
        }

        // POST: ProductController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, ReservationViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bool result = _reservationService.DeleteReservation(id);
                    if (result)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                }

                throw new Exception();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "Ooops! Something went wrong!");
                return View();
            }
        }
    }
}
