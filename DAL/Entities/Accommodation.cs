using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Entities
{
    [Table("Accommodation")]
    public class Accommodation
    {
        [Key]
        public int AccommodationId { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal MonthlyRent { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Size { get; set; } // Square meters

        [Required]
        public int MaxOccupants { get; set; }

        public bool IsAvailable { get; set; } = true;

        // Foreign Keys
        [Required]
        public int LandlordId { get; set; }

        [Required]
        public int AccommodationTypeId { get; set; }

        [Required]
        public int UniversityId { get; set; }

        // Nullable navigation properties
        public virtual Landlord? Landlord { get; set; }
        public virtual AccommodationType? AccommodationType { get; set; }
        public virtual University? University { get; set; }

        // Collections
        public virtual ICollection<AccommodationAmenity> Amenities { get; set; } = new HashSet<AccommodationAmenity>();
        public virtual ICollection<Application> Applications { get; set; } = new HashSet<Application>();
        public virtual ICollection<Booking> Bookings { get; set; } = new HashSet<Booking>();

        // Constructor
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
