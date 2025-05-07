using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Entities
{
    [Table("AccommodationAmenity")]
    public class AccommodationAmenity
    {
        public int AccommodationId { get; set; }
        public int AmenityId { get; set; }

        // Navigation
        public virtual Accommodation Accommodation { get; set; }
        public virtual Amenity Amenity { get; set; }
    }
}