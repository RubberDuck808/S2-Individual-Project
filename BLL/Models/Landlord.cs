using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BLL.Models
{
    public class Landlord
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

        public List<Accommodation> Accommodations { get; set; } = new();


        // Constructor 
        public Landlord(string userId, string name, string email, string phoneNumber)
        {
            UserId = userId ?? throw new ArgumentNullException(nameof(userId));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Email = email ?? throw new ArgumentNullException(nameof(email));
            PhoneNumber = phoneNumber ?? throw new ArgumentNullException(nameof(phoneNumber));
        }

        public Landlord() { }


    }
}