using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs.Accommodation
{
    public class AccommodationUpdateDto
    {
        [Required]
        public int AccommodationId { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        public string Address { get; set; } = string.Empty;

        public decimal MonthlyRent { get; set; }
        public decimal DepositAmount { get; set; }

        public int AreaSqM { get; set; }
        public int MaxOccupants { get; set; }
        public int MinimumLeaseMonths { get; set; }

        public DateTime AvailableFrom { get; set; }

        public int AccommodationTypeId { get; set; }
        public int UniversityId { get; set; }

        public List<int>? AmenityIds { get; set; }
    }
}
