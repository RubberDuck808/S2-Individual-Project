using Microsoft.AspNetCore.Http;
using DAL.Models;
using System.Collections.Generic;
using BLL.DTOs.Accommodation;

namespace UI.ViewModels
{
    using System.ComponentModel.DataAnnotations;

    public class CreateAccommodationViewModel
    {
        [Display(Name = "Title")]
        public string Title { get; set; } = string.Empty;

        [Display(Name = "Description")]
        public string Description { get; set; } = string.Empty;

        [Display(Name = "Address")]
        public string Address { get; set; } = string.Empty;

        [Display(Name = "Monthly Rent (€)")]
        public decimal MonthlyRent { get; set; }

        [Display(Name = "Size (m²)")]
        public decimal Size { get; set; }

        [Display(Name = "Max Occupants")]
        public int MaxOccupants { get; set; }

        [Display(Name = "Accommodation Type")]
        public int AccommodationTypeId { get; set; }

        [Display(Name = "Nearby University")]
        public int UniversityId { get; set; }

        [Display(Name = "Amenities")]
        public List<int> SelectedAmenityIds { get; set; } = new();

        [Display(Name = "Upload Images")]
        public List<IFormFile>? Images { get; set; }

        public List<Amenity> Amenities { get; set; } = new();
        public List<AccommodationType> AccommodationTypes { get; set; } = new();
        public List<University> Universities { get; set; } = new();
    }

}
