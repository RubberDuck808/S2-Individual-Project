using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs.Application
{
    public class ApplicationCreateDto
    {
        [Required]
        public int StudentId { get; set; }

        [Required]
        public int AccommodationId { get; set; }

        [Required]
        public int StatusId { get; set; }
        [Required]
        public DateTime ApplicationDate { get; set; } = DateTime.UtcNow;

    }
}
