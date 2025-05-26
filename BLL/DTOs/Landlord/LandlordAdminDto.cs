using BLL.DTOs.Accommodation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs.Landlord
{
    public class LandlordAdminDto
    {
        public int LandlordId { get; set; }
        public string UserId { get; set; }

        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }

        public string? CompanyName { get; set; }
        public string? TaxIdentificationNumber { get; set; }

        public bool IsVerified { get; set; } = false;
        public DateTime? VerificationDate { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public List<AccommodationDto> Accommodations { get; set; } = new();
    }
}

