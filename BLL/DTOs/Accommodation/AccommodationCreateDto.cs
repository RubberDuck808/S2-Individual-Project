using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs.Accommodation
{
    public class AccommodationCreateDto
    {
        [Required]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        public string Address { get; set; } = string.Empty;
        [Required]
        public string PostCode { get; set; } = string.Empty;
        [Required]
        public string City { get; set; } = string.Empty;

        [Required]
        public string Country { get; set; } = string.Empty;
        [Range(100, 10000)]
        public decimal MonthlyRent { get; set; }

        [Required]
        public decimal Size { get; set; }  // matches `DAL.Models.Accommodation.Size`

        [Range(1, 10)]
        public int MaxOccupants { get; set; } = 1;

        [Required]
        public int LandlordId { get; set; }

        [Required]
        public int UniversityId { get; set; }

        [Required]
        public int AccommodationTypeId { get; set; }

        // Optional: collected from the form as checkboxes
        public List<int> AmenityIds { get; set; } = new();

        // Uploaded image files from form
        public List<IFormFile>? Images { get; set; }
    }
}
