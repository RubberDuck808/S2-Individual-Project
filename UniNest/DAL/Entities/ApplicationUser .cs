using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UniNest.DAL.Entities
{
    [Table("ApplicationUser")]
    public class ApplicationUser : IdentityUser  // Inherits from IdentityUser
    {
        // Extended properties (add your custom fields)
        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;

        // Navigation properties to your domain entities
        public virtual Student? Student { get; set; }
        public virtual Landlord? Landlord { get; set; }
    }
}