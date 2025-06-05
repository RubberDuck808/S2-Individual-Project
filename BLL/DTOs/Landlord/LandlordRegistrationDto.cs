using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs.Landlord
{
    public class LandlordRegistrationDto
    {
        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [StringLength(100)]
        public string? MiddleName { get; set; }

        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(255)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Phone]
        [StringLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        [StringLength(255)]
        public string? CompanyName { get; set; }

        [StringLength(50)]
        public string? TaxNumber { get; set; }
    }
}
