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
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            _logger.LogInformation("Accessed Reservation Index");


            if (userRole=="1")
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

        public IActionResult Index1()
        {
            var userid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            _logger.LogInformation("Accessed Reservation Index");


            if (userRole == "1")
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
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            _logger.LogInformation("Accessed Reservation Index");


            if (userRole == "1")
            {
                return RedirectToAction("Index1", "Reservation");
            }
            else
                return RedirectToAction("Index", "Reservation");
        }


        public IActionResult Add1(int? roomId)
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


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Add1(ReservationViewModel model, int? roomId)
        {
            if (!_reservationService.IsRoomAvailable(model.RoomId, model.ArrivalDate, model.DateOfExit))
            {
                ModelState.AddModelError("RoomId", "This room is not available for the selected dates. Please choose different dates or another room.");

                // Repopulate dropdowns
                ViewBag.Rooms = new SelectList(_context.Rooms, "RoomId", "RoomNo", model.RoomId);
                ViewBag.Customers = new SelectList(_context.Users, "CustomerId", "FullName", model.UserId);
                return View(model);
            }
            var userid = User.FindFirstValue(ClaimTypes.NameIdentifier);
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
            if (model.ArrivalDate > model.DateOfExit)
            {
                ModelState.AddModelError("DateOfExit", "Exit Date cannot be before Arrival Date");
                return View(model);
            }

            if (roleClaim != null && (roleClaim.Value == "1" || string.Equals(roleClaim.Value, "Admin", StringComparison.OrdinalIgnoreCase)))
            {
                return RedirectToAction("Index1", "Reservation");
            }
            else
                return RedirectToAction(nameof(Index));
        }


        public ActionResult Edit(int id)
        {
            var model = _context.Reservations
                .AsNoTracking()
                .Include(r => r.Room)
                .Include(r => r.User)
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
                    if (!_reservationService.IsRoomAvailable(model.RoomId, model.ArrivalDate, model.DateOfExit, model.ReservationId))
                    {
                        ModelState.AddModelError("RoomId",
                            "This room is not available for the selected dates.");

                        ViewBag.Rooms = new SelectList(_context.Rooms, "RoomId", "RoomNo", model.RoomId);
                        ViewBag.Customers = new SelectList(_context.Users, "CustomerId", "FullName", model.UserId);
                        return View(model);
                    }

                    bool result = _reservationService.EditReservation(model);
                    if (result)
                    {
                        var roleClaim = User.FindFirst("RoleId") ?? User.FindFirst(ClaimTypes.Role);
                        if (roleClaim != null && (roleClaim.Value == "1" || string.Equals(roleClaim.Value, "Admin", StringComparison.OrdinalIgnoreCase)))
                        {
                            return RedirectToAction("Index1", "Reservation");
                        }
                        else
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

        public ActionResult Edit1(int id)
        {
            var model = _context.Reservations
                .AsNoTracking()
                .Include(r => r.Room)
                .Include(r => r.User)
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
        public ActionResult Edit1(ReservationViewModel model)
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
                    if (!_reservationService.IsRoomAvailable(model.RoomId, model.ArrivalDate, model.DateOfExit, model.ReservationId))
                    {
                        ModelState.AddModelError("RoomId",
                            "This room is not available for the selected dates.");

                        ViewBag.Rooms = new SelectList(_context.Rooms, "RoomId", "RoomNo", model.RoomId);
                        ViewBag.Customers = new SelectList(_context.Users, "CustomerId", "FullName", model.UserId);
                        return View(model);
                    }

                    bool result = _reservationService.EditReservation(model);
                    if (result)
                    {
                        var roleClaim = User.FindFirst("RoleId") ?? User.FindFirst(ClaimTypes.Role);
                        if (roleClaim != null && (roleClaim.Value == "1" || string.Equals(roleClaim.Value, "Admin", StringComparison.OrdinalIgnoreCase)))
                        {
                            return RedirectToAction("Index1", "Reservation");
                        }
                        else
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

        // GET: Reservation/Delete/5
        [Authorize]
        public ActionResult Delete(int id)
        {
            try
            {
                var reservation = _context.Reservations
                    .Include(r => r.User)
                    .Include(r => r.Room)
                    .AsNoTracking()
                    .FirstOrDefault(r => r.ReservationId == id);

                if (reservation == null)
                {
                    _logger.LogWarning("Delete called for non-existent reservation id {ReservationId}", id);
                    TempData["ErrorMessage"] = "Reservation not found.";
                    var roleClaim = User.FindFirst("RoleId") ?? User.FindFirst(ClaimTypes.Role);
                    if (roleClaim != null && (roleClaim.Value == "1" || string.Equals(roleClaim.Value, "Admin", StringComparison.OrdinalIgnoreCase)))
                    {
                        return RedirectToAction("Index1", "Reservation");
                    }
                    else
                        return RedirectToAction(nameof(Index));
                }

                var model = new ReservationViewModel
                {
                    ReservationId = reservation.ReservationId,
                    ReservationDate = reservation.ReservationDate,
                    ArrivalDate = reservation.ArrivalDate,
                    DateOfExit = reservation.DateOfExit,
                    ModeOfOrder = reservation.ModeOfOrder,
                    WithCar = reservation.WithCar,
                    CarRegNo = reservation.CarRegNo,
                    User = reservation.User,
                    Room = reservation.Room
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading delete view for reservation {ReservationId}", id);
                TempData["ErrorMessage"] = "An error occurred while loading the reservation.";
                var roleClaim = User.FindFirst("RoleId") ?? User.FindFirst(ClaimTypes.Role);
                if (roleClaim != null && (roleClaim.Value == "1" || string.Equals(roleClaim.Value, "Admin", StringComparison.OrdinalIgnoreCase)))
                {
                    return RedirectToAction("Index1", "Reservation");
                }
                else
                    return RedirectToAction(nameof(Index));
            }
        }

        // POST: Reservation/Delete/5
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, ReservationViewModel model)
        {
            try
            {
                bool result = _reservationService.DeleteReservation(id);

                if (result)
                {
                    TempData["SuccessMessage"] = "Reservation deleted successfully.";
                    _logger.LogInformation("Deleted reservation with ID {ReservationId}", id);
                    var roleClaim = User.FindFirst("RoleId") ?? User.FindFirst(ClaimTypes.Role);
                    if (roleClaim != null && (roleClaim.Value == "1" || string.Equals(roleClaim.Value, "Admin", StringComparison.OrdinalIgnoreCase)))
                    {
                        return RedirectToAction("Index1", "Reservation");
                    }
                    else
                        return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to delete reservation. It may have already been deleted.";
                    var roleClaim = User.FindFirst("RoleId") ?? User.FindFirst(ClaimTypes.Role);
                    if (roleClaim != null && (roleClaim.Value == "1" || string.Equals(roleClaim.Value, "Admin", StringComparison.OrdinalIgnoreCase)))
                    {
                        return RedirectToAction("Index1", "Reservation");
                    }
                    else
                        return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting reservation {ReservationId}", id);
                TempData["ErrorMessage"] = "An error occurred while deleting the reservation.";
                var roleClaim = User.FindFirst("RoleId") ?? User.FindFirst(ClaimTypes.Role);
                if (roleClaim != null && (roleClaim.Value == "1" || string.Equals(roleClaim.Value, "Admin", StringComparison.OrdinalIgnoreCase)))
                {
                    return RedirectToAction("Index1", "Reservation");
                }
                else
                    return RedirectToAction(nameof(Index));
            }
        }

        [Authorize]
        public ActionResult Delete1(int id)
        {
            try
            {
                var reservation = _context.Reservations
                    .Include(r => r.User)
                    .Include(r => r.Room)
                    .AsNoTracking()
                    .FirstOrDefault(r => r.ReservationId == id);

                if (reservation == null)
                {
                    _logger.LogWarning("Delete called for non-existent reservation id {ReservationId}", id);
                    TempData["ErrorMessage"] = "Reservation not found.";
                    var roleClaim = User.FindFirst("RoleId") ?? User.FindFirst(ClaimTypes.Role);
                    if (roleClaim != null && (roleClaim.Value == "1" || string.Equals(roleClaim.Value, "Admin", StringComparison.OrdinalIgnoreCase)))
                    {
                        return RedirectToAction("Index1", "Reservation");
                    }
                    else
                        return RedirectToAction(nameof(Index)); ;
                }

                var model = new ReservationViewModel
                {
                    ReservationId = reservation.ReservationId,
                    ReservationDate = reservation.ReservationDate,
                    ArrivalDate = reservation.ArrivalDate,
                    DateOfExit = reservation.DateOfExit,
                    ModeOfOrder = reservation.ModeOfOrder,
                    WithCar = reservation.WithCar,
                    CarRegNo = reservation.CarRegNo,
                    User = reservation.User,
                    Room = reservation.Room
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading delete view for reservation {ReservationId}", id);
                TempData["ErrorMessage"] = "An error occurred while loading the reservation.";
                var roleClaim = User.FindFirst("RoleId") ?? User.FindFirst(ClaimTypes.Role);
                if (roleClaim != null && (roleClaim.Value == "1" || string.Equals(roleClaim.Value, "Admin", StringComparison.OrdinalIgnoreCase)))
                {
                    return RedirectToAction("Index1", "Reservation");
                }else
                    return RedirectToAction(nameof(Index));
            }
        }

        // POST: Reservation/Delete/5
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Delete1(int id, ReservationViewModel model)
        {
            try
            {
                bool result = _reservationService.DeleteReservation(id);

                if (result)
                {
                    TempData["SuccessMessage"] = "Reservation deleted successfully.";
                    _logger.LogInformation("Deleted reservation with ID {ReservationId}", id);
                    var roleClaim = User.FindFirst("RoleId") ?? User.FindFirst(ClaimTypes.Role);
                    if (roleClaim != null && (roleClaim.Value == "1" || string.Equals(roleClaim.Value, "Admin", StringComparison.OrdinalIgnoreCase)))
                    {
                        return RedirectToAction("Index1", "Reservation");
                    }
                    else
                        return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to delete reservation. It may have already been deleted.";
                    var roleClaim = User.FindFirst("RoleId") ?? User.FindFirst(ClaimTypes.Role);
                    if (roleClaim != null && (roleClaim.Value == "1" || string.Equals(roleClaim.Value, "Admin", StringComparison.OrdinalIgnoreCase)))
                    {
                        return RedirectToAction("Index1", "Reservation");
                    }
                    else
                        return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting reservation {ReservationId}", id);
                TempData["ErrorMessage"] = "An error occurred while deleting the reservation.";
                var roleClaim = User.FindFirst("RoleId") ?? User.FindFirst(ClaimTypes.Role);
                if (roleClaim != null && (roleClaim.Value == "1" || string.Equals(roleClaim.Value, "Admin", StringComparison.OrdinalIgnoreCase)))
                {
                    return RedirectToAction("Index1", "Reservation");
                }
                else
                    return RedirectToAction(nameof(Index));
            }
        }
    }
}
