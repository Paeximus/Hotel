using Hotel.Models.Data.HotelContext;
using Hotel.Models.ViewModels;

namespace Hotel.Services
{
    public class FloorServices
    {
        private readonly HotelContext _context;
        private readonly ILogger<FloorServices> _logger;
        public FloorServices(HotelContext context, ILogger<FloorServices> logger)
        {
            _context = context;
            _logger = logger;
        }
        public bool AddFloor(FloorViewModel model)
        {
            try
            {
                Floor floor = new Floor
                {
                    FloorNo = model.FloorNo
                };
                _context.Floors.Add(floor);
                _context.SaveChanges();
                _logger.LogInformation("Added new floor with ID {FloorId}", floor.FloorId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AddFloor failed for floor number {FloorNumber}", model.FloorNo);
                return false;
            }
        }
    }
}
