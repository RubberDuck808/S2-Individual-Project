using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Entities
{
    [Table("Application")]
    public class Application
    {
        public int ApplicationId { get; set; }

        // Foreign keys
        public int StudentId { get; set; }
        public int AccommodationId { get; set; }

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Pending"; 

        public DateTime ApplicationDate { get; set; } = DateTime.UtcNow;

        public int StatusId { get; set; }
        public virtual Status StatusNavigation { get; set; }

        // Navigation
        public virtual Student Student { get; set; }
        public virtual Accommodation Accommodation { get; set; }
    }
}
