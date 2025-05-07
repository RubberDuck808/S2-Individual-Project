using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Entities
{
    [Table("AccommodationType")]
    public class AccommodationType
    {
        [Key]
        public int AccommodationTypeId { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        // Navigation
        public virtual ICollection<Accommodation> Accommodations { get; set; } = new HashSet<Accommodation>();
    }
}