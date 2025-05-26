using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Models
{
    public class AccommodationAmenity
    {
        public int AccommodationId { get; set; }
        public int AmenityId { get; set; }

    }
}