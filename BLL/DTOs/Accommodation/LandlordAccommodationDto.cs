// BLL/DTOs/Accommodation/LandlordAccommodationDto.cs
namespace BLL.DTOs.Accommodation
{
    public class LandlordAccommodationDto
    {
        public int AccommodationId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string AccommodationType { get; set; }
        public string Address { get; set; }
        public string PostCode { get; set; }
        public string City { get; set; }
        public bool IsAvailable { get; set; }
        public string Country { get; set; }
        public decimal MonthlyRent { get; set; }
        public int Size { get; set; }
        public int MaxOccupants { get; set; }
        public DateTime AvailableFrom { get; set; }

        public string UniversityName { get; set; }
        public List<string> ImageUrls { get; set; } = new();
        public List<string> AmenityNames { get; set; } = new();

        // Landlord-only info
        public int ApplicationCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }

    }
}
