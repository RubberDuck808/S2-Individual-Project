public class AccommodationDto
{
    public int AccommodationId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public decimal MonthlyRent { get; set; }
    public bool IsAvailable { get; set; }

    public int MaxOccupants { get; set; }
    public int AreaSqM { get; set; }
    public DateTime AvailableFrom { get; set; }

    // Related info 
    public string AccommodationType { get; set; } = string.Empty;
    public string UniversityName { get; set; } = string.Empty;
    public string LandlordName { get; set; } = string.Empty;

    public List<string> AmenityNames { get; set; } = new();
    public List<string> ImageUrls { get; set; } = new();
}
