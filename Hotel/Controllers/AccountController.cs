using Hotel.Models.Data.HotelContext;
using Hotel.Models.ViewModels;
using Hotel.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Hotel.Controllers
{
    public class AccountController : Controller
    {
        private readonly HotelContext _context;
        private readonly AccountServices _accountService;
        private readonly ILogger<AccountController> _logger;
        public AccountController(HotelContext context, AccountServices accountService, ILogger<AccountController> logger)
        {
            _context = context;
            _accountService = accountService;
            _logger = logger;
        }

        // GET: AccountController
        public ActionResult Index()
        {
            return View();
        }

        // GET: AccountController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: AccountController/Create
        public ActionResult Register()
        {

            return View();
        }

        // POST: AccountController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(AccountViewModel model)
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
                var existingUser = _context.Users.
                    FirstOrDefault(u => u.Email == model.Email);

                if (existingUser != null)
                {
                    _logger.LogWarning("Registration failed - Email already in use");
                    ModelState.AddModelError("Email", "Email already in use");
                    return View(model);
                }

                bool result = _accountService.AddUser(model);
                if (!result)
                {
                    _logger.LogError("Registration failed - Service error");
                    ModelState.AddModelError(string.Empty, "An error occurred while registering the user.");
                    return View(model);
                }
                else
                {
                    _logger.LogInformation("User registered successfully");
                }
                if (model.RoleId == "1")
                {
                    return RedirectToAction("Dashboard", "Admin");
                }
                else
                {
                    return RedirectToAction("Dashboard", "Client");
                }
            }
            catch
            {
                return View();
            }
        }

        // GET: AccountController/Edit/5
        public ActionResult Login()
        {
            return View();
        }

        // POST: AccountController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            _logger.LogInformation("Login attempt for {Email}", model.Email);
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Login failed - invalid model state");
                return View(model);
            }

            var user = await _context.Users.
                FirstOrDefaultAsync(u => u.Email == model.Email);
            if (user == null)
            {
                _logger.LogWarning("Login failed - user not found for {Email}", model.Email);
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(model);
            }

            var hasher = new PasswordHasher<User>();
            var verify = hasher.VerifyHashedPassword(user, user.Password, model.Password);

            if (verify != PasswordVerificationResult.Success)
            {
                _logger.LogWarning("Login failed - invalid password for {Email}", model.Email);
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(model);
            }

            HttpContext.Session.SetInt32("UserId", user.UserId);
            HttpContext.Session.SetString("RoleId", user.RoleId);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.FirstName ?? user.Email),
                new Claim(ClaimTypes.Role, user.RoleId.ToString()) // important for [Authorize(Roles="1")]
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            _logger.LogInformation("Login successful for {Email}", model.Email);
            if (user.RoleId == "1")
            {
                return RedirectToAction("Dashboard", "Admin");
            }
            else if(user.RoleId == "2")
            {
                return RedirectToAction("Dashboard", "Client");
            }

            return View(model);

        }

        // GET: AccountController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: AccountController/Delete/5
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
