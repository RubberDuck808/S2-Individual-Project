using System.ComponentModel.DataAnnotations;

namespace UniNest.BLL.DTOs.Landlord
{
    public class LandlordDto
    {
        public int LandlordId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Phone]
        public string PhoneNumber { get; set; } = string.Empty;

        [StringLength(100)]
        public string? CompanyName { get; set; }

        public bool IsVerified { get; set; }
        public int ActiveListingsCount { get; set; }
        public decimal TotalMonthlyRent { get; set; }

        public string DisplayName => CompanyName ?? Name;
    }
}
