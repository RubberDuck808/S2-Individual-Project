using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UniNest.DAL.Entities
{
    [Table("Status")]
    public class Status
    {
        [Key]
        public int StatusId { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; } 
    }
}