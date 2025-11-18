using Hotel.Models.Data.HotelContext;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hotel.Models.ViewModels
{
    public class RoomViewModel
    {
        public int RoomId { get; set; }

        public string RoomNo { get; set; } = null!;

        public int MaxOccupants { get; set; }

        public string IsOccupied { get; set; } = null!;

        public decimal Price { get; set; }

        public int FloorId { get; set; }

        // persisted image bytes
        public byte[] RoomImage { get; set; } = Array.Empty<byte>();

        // file uploaded from the form
        [ValidateNever]
        [NotMapped]
        public IFormFile? RoomImageFile { get; set; }

        [ValidateNever]
        public virtual Floor? Floor { get; set; } = null!;
    }
}
