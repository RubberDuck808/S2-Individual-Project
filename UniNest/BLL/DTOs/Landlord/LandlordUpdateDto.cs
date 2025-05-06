using System.ComponentModel.DataAnnotations;

namespace UniNest.BLL.DTOs.Landlord
{
    public class LandlordUpdateDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Phone]
        [StringLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;

        [StringLength(100)]
        public string? CompanyName { get; set; }

        [StringLength(50)]
        public string? TaxIdentificationNumber { get; set; }
    }
}
