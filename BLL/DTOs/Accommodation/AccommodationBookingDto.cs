using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs.Accommodation
{
    public class AccommodationBookingDto
    {
        public int BookingId { get; set; }
        public int AccommodationId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string PostCode { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public decimal MonthlyRent { get; set; }
        public bool IsAvailable { get; set; }
        public int MaxOccupants { get; set; }
        public decimal Size { get; set; }
        public DateTime AvailableFrom { get; set; }
        public string AccommodationType { get; set; } = string.Empty;
        public string UniversityName { get; set; } = string.Empty;
        public List<string> AmenityNames { get; set; } = new();
        public List<string> ImageUrls { get; set; } = new();
        public string BookingStatus { get; set; } = string.Empty;
    }

}
