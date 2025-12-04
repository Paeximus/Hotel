using Hotel.Models.Data.HotelContext;
using Hotel.Models.ViewModels;
using Hotel.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.IO;
using System.Threading.Tasks;

namespace Hotel.Controllers
{
    public class RoomController : Controller
    {
        private readonly RoomServices _roomService;
        private readonly HotelContext _context;
        private readonly ILogger<RoomController> _logger;

        public RoomController(RoomServices roomService, ILogger<RoomController> logger, HotelContext context)
        {
            _roomService = roomService;
            _logger = logger;
            _context = context;
        }

        // GET: RoomController
        public ActionResult Index()
        {
            var rooms = _roomService.GetUnreservedRooms();
            _logger.LogInformation("Accessed RoomController Index action.");

            return View(rooms);
        }

        public ActionResult Index1()
        {
            var rooms = _roomService.GetRooms();
            _logger.LogInformation("Accessed RoomController Index action.");

            return View(rooms);
        }

        // GET: RoomController/Details/5
        public ActionResult Details1(int id)
        {
            var model = _roomService.GetRoom(id);
            return View(model);
        }

        // GET: RoomController/Create
        public ActionResult Add()
        {
            // Populate floors for the select list
            var floors = _context.Floors.OrderBy(f => f.FloorNo).ToList();
            ViewBag.Floors = new SelectList(floors, "FloorId", "FloorName");
            return View();
        }

        // POST: RoomController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(RoomViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // repopulate floors before returning the view
                var floors = _context.Floors.OrderBy(f => f.FloorNo).ToList();
                ViewBag.Floors = new SelectList(floors, "FloorId", "FloorName");

                var errors = ModelState
                    .Where(kv => kv.Value.Errors.Count > 0)
                    .Select(kv =>
                        new
                        {
                            Key = kv.Key,
                            Errors = kv.Value.Errors.Select(e => string.IsNullOrEmpty(e.ErrorMessage) ? (e.Exception?.Message ?? "<no message>") : e.ErrorMessage).ToArray()
                        });

                foreach (var e in errors)
                    _logger.LogWarning("ModelState error for {Key}: {Errors}", e.Key, string.Join(" | ", e.Errors));

                return View(model);
            }

            try
            {
                _roomService.AddRoom(model);
                return RedirectToAction("Index1", "Room");
            }
            catch (Exception ex)
            {
                // repopulate floors so view can render after error
                var floors = _context.Floors.OrderBy(f => f.FloorNo).ToList();
                ViewBag.Floors = new SelectList(floors, "FloorId", "FloorName");

                _logger.LogError(ex, "Failed to add room");
                return View(model);
            }
        }

        // GET: RoomController/Edit/5
        public ActionResult Edit(int id)
        {
            var room = _context.Rooms
                .AsNoTracking()
                .Include(r => r.Floor)
                .FirstOrDefault(r => r.RoomId == id);

            if (room == null)
            {
                _logger.LogWarning("Edit called for non-existent room id {RoomId}", id);
                return NotFound();
            }

            var model = new RoomViewModel
            {
                RoomId = room.RoomId,
                RoomNo = room.RoomNo,
                MaxOccupants = room.MaxOccupants,
                IsOccupied = room.IsOccupied,
                Price = room.Price,
                FloorId = room.FloorId,
                RoomImage = room.RoomImage ?? Array.Empty<byte>(),
            };

            var floors = _context.Floors.OrderBy(f => f.FloorNo).ToList();
            ViewBag.Floors = new SelectList(floors, "FloorId", "FloorName", model.FloorId);

            return View(model);
        }

        // POST: RoomController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, RoomViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // If a new file was uploaded, read it into the model; otherwise preserve existing bytes
                    if (model.RoomImageFile != null && model.RoomImageFile.Length > 0 )
                    {
                        using var ms = new MemoryStream();
                        model.RoomImageFile.CopyTo(ms);
                        model.RoomImage = ms.ToArray();
                    }
                    else
                    {
                        // preserve existing image bytes from DB if any
                        var existing = _context.Rooms.AsNoTracking().FirstOrDefault(r => r.RoomId == model.RoomId);
                        model.RoomImage = existing?.RoomImage ?? Array.Empty<byte>();
                    }

                    bool result = _roomService.EditRoom(model);
                    if (result)
                    {
                        return RedirectToAction(nameof(Index1));
                    }

                    ModelState.AddModelError(string.Empty, "Could not save changes. Try again.");
                }

                var floors = _context.Floors.OrderBy(f => f.FloorNo).ToList();
                ViewBag.Floors = new SelectList(floors, "FloorId", "FloorName", model.FloorId);
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Edit failed for room id {RoomId}", id);
                ModelState.AddModelError("Error", "Ooops! Something went wrong!");
                var floors = _context.Floors.OrderBy(f => f.FloorNo).ToList();
                ViewBag.Floors = new SelectList(floors, "FloorId", "FloorName", model?.FloorId ?? 0);
                return View(model);
            }
        }

        // GET: RoomController/Delete/5
        public ActionResult Delete(int id)
        {
            var model = _roomService.GetRoom(id);
            return View(model);
        }

        // POST: ProductController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, RoomViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bool result = _roomService.DeleteRoom(id);
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
