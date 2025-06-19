using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs.Accommodation
{
    public class AccommodationUpdateDto
    {
        public int AccommodationId { get; set; }
        public string Title { get; set; } = string.Empty;

        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string PostCode { get; set; }
        public string City { get; set; }
        public string Country { get; set; }

        public decimal MonthlyRent { get; set; }
        public bool IsAvailable { get; set; }

        public int MaxOccupants { get; set; }
        public decimal Size { get; set; }
        public DateTime AvailableFrom { get; set; }



        // Related info 
        public string AccommodationType { get; set; } = string.Empty;
        public int UniversityId { get; set; }
        public int AccommodationTypeId { get; set; }
        public string UniversityName { get; set; } = string.Empty;
        public string LandlordName { get; set; } = string.Empty;

        public List<string> AmenityNames { get; set; } = new();
        public List<string> ImageUrls { get; set; } = new();
        public int LandlordId { get; set; }
    }
}
