using System.ComponentModel.DataAnnotations;

public class LandlordUpdateDto
{
    [Required]
    [StringLength(100)]
    public string FirstName { get; set; }
    [StringLength(100)]
    public string? MiddleName { get; set; }

    [Required]
    [StringLength(100)]
    public string LastName { get; set; }

    [Required]
    [EmailAddress]
    [StringLength(100)]
    public string Email { get; set; }

    [Required]
    [Phone]
    [StringLength(20)]
    public string PhoneNumber { get; set; }

    [StringLength(100)]
    public string? CompanyName { get; set; }

    [StringLength(50)]
    public string? TaxIdentificationNumber { get; set; }
}
