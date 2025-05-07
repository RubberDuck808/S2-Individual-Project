using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Entities
{
    [Table("Landlord")]
    public class Landlord
    {
        // Primary Key
        [Key]
        public int LandlordId { get; set; }


        [Required]
        [StringLength(450)] 
        public string UserId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(255)]
        public string Email { get; set; }

        [Required]
        [Phone]
        [StringLength(20)]
        public string PhoneNumber { get; set; }


        [StringLength(100)]
        public string? CompanyName { get; set; }

        [StringLength(50)]
        public string? TaxIdentificationNumber { get; set; }

        public bool IsVerified { get; set; } = false;
        public DateTime? VerificationDate { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        [ForeignKey(nameof(UserId))]
        public virtual ApplicationUser User { get; set; }

        public virtual ICollection<Accommodation> Accommodations { get; set; } = new HashSet<Accommodation>();

        // Constructor 
        public Landlord(string userId, string name, string email, string phoneNumber)
        {
            UserId = userId ?? throw new ArgumentNullException(nameof(userId));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Email = email ?? throw new ArgumentNullException(nameof(email));
            PhoneNumber = phoneNumber ?? throw new ArgumentNullException(nameof(phoneNumber));
        }

        // Business Methods
        public bool CanAddMoreProperties()
        {
            const int maxPropertiesForIndividual = 5;
            return CompanyName == null
                ? Accommodations.Count < maxPropertiesForIndividual
                : true;
        }
    }
}