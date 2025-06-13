using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models
{

    public class AccommodationImage
    {
        public int ImageId { get; set; }

        public int AccommodationId { get; set; }

        public string ImageUrl { get; set; } = string.Empty;


        public string? Description { get; set; }

        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

    }
}
