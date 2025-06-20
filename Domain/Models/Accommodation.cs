﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography.X509Certificates;

namespace Domain.Models
{
    public class Accommodation
    {
        public int AccommodationId { get; set; }

        public string Title { get; set; } = string.Empty;

        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        public string Description { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;
        public string PostCode { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;

        public string Country { get; set; } = "Netherlands"; // default if applicable



        public decimal MonthlyRent { get; set; }

        public decimal Size { get; set; }

        public int MaxOccupants { get; set; }

        public bool IsAvailable { get; set; } = true;

        public int LandlordId { get; set; }

        public int AccommodationTypeId { get; set; }

        public int UniversityId { get; set; }
        public DateTime AvailableFrom { get; set; }



        // Optional: Constructor
        public Accommodation(string title, string description, decimal monthlyRent,
                             int size, int maxOccupants, int landlordId,
                             int accommodationTypeId, int universityId)
        {
            Title = title;
            Description = description;
            MonthlyRent = monthlyRent;
            Size = size;
            MaxOccupants = maxOccupants;
            LandlordId = landlordId;
            AccommodationTypeId = accommodationTypeId;
            UniversityId = universityId;
        }

        public Accommodation() { }
    }
}
