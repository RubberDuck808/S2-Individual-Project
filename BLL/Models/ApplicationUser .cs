using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BLL.Models
{
    public class ApplicationUser
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        [Required]
        public string UserName { get; set; } = string.Empty;

        public string NormalizedUserName { get; set; } = string.Empty;

        [Required]
        public string Email { get; set; } = string.Empty;

        public string NormalizedEmail { get; set; } = string.Empty;

        public bool EmailConfirmed { get; set; } = false;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        public string SecurityStamp { get; set; } = Guid.NewGuid().ToString();

        public string ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();

        public string PhoneNumber { get; set; } = string.Empty;

        public bool PhoneNumberConfirmed { get; set; } = false;

        public bool TwoFactorEnabled { get; set; } = false;

        public DateTime? LockoutEnd { get; set; }

        public bool LockoutEnabled { get; set; } = false;

        public int AccessFailedCount { get; set; } = 0;
    }
}
