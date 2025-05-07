using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Entities
{
    [Table("Booking")]
    public class Booking
    {
        [Key]
        public int BookingId { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalAmount { get; set; }

        // Foreign Keys
        [Required]
        public int StudentId { get; set; }

        [Required]
        public int AccommodationId { get; set; }

        [Required]
        public int StatusId { get; set; }

        // Navigation
        public virtual Student Student { get; set; }
        public virtual Accommodation Accommodation { get; set; }
        public virtual Status Status { get; set; }
    }
}