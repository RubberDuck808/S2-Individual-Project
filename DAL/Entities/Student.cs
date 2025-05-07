using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static System.Net.Mime.MediaTypeNames;

namespace DAL.Entities
{
    [Table("Student")]
    public class Student
    {
        [Key]
        public int StudentId { get; set; }

        [Required]
        [StringLength(450)] 
        public string UserId { get; set; }

        [Required]
        [ForeignKey(nameof(University))]
        public int UniversityId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [Phone]
        [StringLength(20)]
        public string PhoneNumber { get; set; }

        [StringLength(100)]
        public string EmergencyContact { get; set; }

        [Phone]
        [StringLength(20)]
        public string EmergencyPhone { get; set; }

        [Url]
        [StringLength(255)]
        public string ProfileImageUrl { get; set; }

        public bool IsVerified { get; set; } = false;

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual University University { get; set; }
        public virtual ApplicationUser User { get; set; }
        public virtual ICollection<Application> Applications { get; set; } = new HashSet<Application>();

        // Constructor for required fields
        public Student(string userId, int universityId, DateTime dateOfBirth, string phoneNumber)
        {
            UserId = userId ?? throw new ArgumentNullException(nameof(userId));
            UniversityId = universityId;
            DateOfBirth = dateOfBirth;
            PhoneNumber = phoneNumber ?? throw new ArgumentNullException(nameof(phoneNumber));
        }
    }
}