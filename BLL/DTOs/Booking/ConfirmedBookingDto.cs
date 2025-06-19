using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs.Booking
{
    public class ConfirmedBookingDto
    {
        public int BookingId { get; set; }
        public int AccommodationId { get; set; }
        public string AccommodationTitle { get; set; }
        public int StudentId { get; set; }
        public string BookingStatus { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalAmount { get; set; }
        public List<string> ImageUrls { get; set; } = new();
    }

}
