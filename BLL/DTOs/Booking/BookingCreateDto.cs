using System;
using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs.Booking
{
    public class BookingCreateDto
    {
        [Required]
        public int StudentId { get; set; }

        [Required]
        public int AccommodationId { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Range(0, 10000)]
        public decimal TotalAmount { get; set; }

        [Required]
        public int StatusId { get; set; } 
    }
}
