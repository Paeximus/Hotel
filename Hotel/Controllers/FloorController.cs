using Hotel.Models.ViewModels;
using Hotel.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Hotel.Controllers
{
    public class FloorController : Controller
    {
        private readonly FloorServices _floorService;
        private readonly ILogger<FloorController> _logger;
        public FloorController(FloorServices floorService, ILogger<FloorController> logger)
        {
            _floorService = floorService;
            _logger = logger;
        }
        // GET: FloorController
        public ActionResult Index()
        {
            return View();
        }

        // GET: FloorController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: FloorController/Create
        public ActionResult Add()
        {
            return View();
        }

        // POST: FloorController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(FloorViewModel model)
        {
            if (!ModelState.IsValid)
            {
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
                _floorService.AddFloor(model);

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: FloorController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: FloorController/Edit/5
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

        // GET: FloorController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: FloorController/Delete/5
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
