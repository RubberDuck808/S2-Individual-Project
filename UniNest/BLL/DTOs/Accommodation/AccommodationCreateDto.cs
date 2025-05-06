using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UniNest.BLL.DTOs.Accommodation
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

        [Range(300, 5000)]
        public decimal MonthlyRent { get; set; }

        public decimal DepositAmount { get; set; } = 0;

        [Required]
        public int AreaSqM { get; set; }

        public int MaxOccupants { get; set; } = 1;
        public int MinimumLeaseMonths { get; set; } = 6;

        public DateTime AvailableFrom { get; set; }

        [Required]
        public int LandlordId { get; set; }

        [Required]
        public int UniversityId { get; set; }

        [Required]
        public int AccommodationTypeId { get; set; }

        public List<IFormFile>? Photos { get; set; }
        public List<int>? AmenityIds { get; set; }
    }
}
