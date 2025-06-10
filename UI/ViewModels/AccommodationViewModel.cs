using BLL.DTOs.Accommodation;
using BLL.DTOs.Shared;

namespace UI.ViewModels;
public class AccommodationViewModel
{
    public int AccommodationId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string PostCode { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public decimal MonthlyRent { get; set; }
    public decimal Size { get; set; }
    public int MaxOccupants { get; set; }
    public int AccommodationTypeId { get; set; }
    public int UniversityId { get; set; }
    public int SelectedAccommodationTypeId { get; set; }
    public List<int> SelectedAmenityIds { get; set; } = new();
    public List<IFormFile>? Images { get; set; }

    public List<UniversityDto> Universities { get; set; } = new();
    public List<AccommodationTypeDto> AccommodationTypes { get; set; } = new();
    public List<AmenityDto> Amenities { get; set; } = new();
}
