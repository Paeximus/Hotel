using Hotel.Models.Data.HotelContext;
using Hotel.Models.ViewModels;

namespace Hotel.Services
{
    public class RoleServices
    {
        private readonly RoleViewModel model1;
        private readonly HotelContext _context;
        private ILogger<RoleServices> _logger;
        public RoleServices(HotelContext context, ILogger<RoleServices> logger)
        {
            _context = context;
            _logger = logger;
        }

        public bool AddRole(RoleViewModel model)
        {
            try
            {
                Role role = new Role
                {
                    RoleId = model.RoleId,
                    RoleName = model.RoleName
                };
                _context.Roles.Add(role);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding role");
                return false;
            }
        }   
    }
}
