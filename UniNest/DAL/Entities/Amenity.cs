using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UniNest.DAL.Entities
{
    [Table("Amenity")]
    public class Amenity
    {
        [Key]
        public int AmenityId { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; } 

        // Navigation (for many-to-many with Accommodation)
        public virtual ICollection<AccommodationAmenity> AccommodationAmenities { get; set; } = new HashSet<AccommodationAmenity>();
    }
}