using System;

namespace BLL.DTOs.Booking
{
    public class BookingDto
    {
        public int BookingId { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalAmount { get; set; }

        public string Status { get; set; } = string.Empty;
        public int StatusId { get; set; }

        public int StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;

        public int AccommodationId { get; set; }
        public string AccommodationTitle { get; set; } = string.Empty;
    }
}
