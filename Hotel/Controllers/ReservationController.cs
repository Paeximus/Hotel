using Hotel.Models.ViewModels;
using Hotel.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Hotel.Controllers
{
    [Authorize]
    public class ReservationController : Controller
    {
        private readonly ReservationService _reservationService;
        private readonly ILogger<ReservationController> _logger;

        public ReservationController(ReservationService reservationService, ILogger<ReservationController> logger)
        {
            _reservationService = reservationService;
            _logger = logger;
        }

        // GET: Reservation/Add?roomId=123
        public IActionResult Add(int? roomId)
        {

            ReservationViewModel model = new ReservationViewModel();
            if (roomId.HasValue)
            {
                // Pre-fill the model with roomId so view will render hidden input
                model.RoomId = roomId.Value;
                ViewBag.SelectedRoomId = roomId.Value;
            }
            else
            {
                ViewBag.SelectedRoomId = null;
                // Optionally populate ViewBag.RoomId with SelectList if you want to allow selecting a room.
                // Example (requires a room service): ViewBag.RoomId = new SelectList(_roomService.GetAll(), "RoomId", "RoomNo");
            }

            return View(model);
        }

        // POST: Reservation/Add
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Add(ReservationViewModel model, int? roomId)
        {
            if (!ModelState.IsValid)
            {
                Console.WriteLine("Model state is invalid");
                // Preserve SelectedRoomId so view can re-render correctly
                ViewBag.SelectedRoomId = roomId ?? (object?)null;
                return View(model);
            }

            // Ensure RoomId is taken from the route/query or from server-side logic — do not trust client input alone.
            if (roomId.HasValue)
            {
                model.RoomId = roomId.Value;
                Console.WriteLine($"Using roomId: {model.RoomId}");
            }
            else
                Console.WriteLine("No roomId provided in query or route; using model.RoomId from form data.");

            // Get logged-in user's id from claims
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("Attempt to add reservation without user id");
                return Challenge();
            }

            // Determine role and set ModeOfOrder accordingly:
            // If RoleId == 1 => "Desk", otherwise "Online".
            // Supports numeric RoleId claim ("RoleId"), numeric role in ClaimTypes.Role, or name "Admin".
            string modeOfOrder;
            var roleClaim = User.FindFirst("RoleId") ?? User.FindFirst(ClaimTypes.Role);
            if (roleClaim != null)
            {
                if (int.TryParse(roleClaim.Value, out var roleIdNumeric))
                {
                    modeOfOrder = roleIdNumeric == 1 ? "Client" : "Online";
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
            if (!created)
            {
                ModelState.AddModelError("", "Could not create reservation. See logs.");
                ViewBag.SelectedRoomId = roomId ?? (object?)null;
                return View(model);
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
