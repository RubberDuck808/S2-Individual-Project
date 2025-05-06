using System;
using System.ComponentModel.DataAnnotations;

namespace UniNest.BLL.DTOs.Booking
{
    public class BookingUpdateDto
    {
        [Required]
        public int BookingId { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public int? StatusId { get; set; }
    }
}
