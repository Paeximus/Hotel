using Hotel.Models.Data.HotelContext;
using Hotel.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace Hotel.Services
{
    public class AccountServices
    {
        private readonly HotelContext _context;
        private readonly PasswordHasher<User> hasher = new PasswordHasher<User>();
        private readonly ILogger<AccountServices> _logger;
        public AccountServices(HotelContext context, ILogger<AccountServices> logger) 
        {
            _logger = logger;
            _context = context;
        }

        public bool AddUser(AccountViewModel model)
        {
            try
            {
                User user = new User
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    OtherNames = model.OtherNames,
                    Gender = model.Gender,
                    DateOfBirth = model.DateOfBirth,                    
                    Email = model.Email,
                    RoleId = model.RoleId,                    
                };
                

                user.Password = hasher.HashPassword(user, model.Password);
                _context.Users.Add(user);
                _logger.LogDebug("About to SaveChanges for user {Email}", model.Email);
                _context.SaveChanges();
                _logger.LogInformation("Saved user {Email} with id {Id}", user.Email, user.UserId);

                return true;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AddUser failed for {Email}", model?.Email);
                return false;
            }
        }

        public User? Login(LoginViewModel model)
        {
            try
            {
                var user = _context.Users.FirstOrDefault(u => u.Email == model.Email);
                if (user == null)
                {
                    _logger.LogWarning("Login failed - user not found for {Email}", model.Email);
                    return null;
                }

                var verify = hasher.VerifyHashedPassword(user, user.Password, model.Password);
                if (verify == PasswordVerificationResult.Success)
                    return user;

                _logger.LogWarning("Login failed - invalid password for {Email}", model.Email);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception during Login for {Email}", model.Email);
                return null;
            }
        }
    }
}
