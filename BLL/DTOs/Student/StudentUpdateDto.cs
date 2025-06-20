using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs.Student
{
    public class StudentUpdateDto
    {
        [Required]
        public string FirstName { get; set; } = string.Empty;

        public string? MiddleName { get; set; } // Optional

        [Required]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        public DateTime DateOfBirth { get; set; }

        public string? EmergencyContact { get; set; } // Optional

        [Phone]
        public string? EmergencyPhone { get; set; } // Optional

        [Required]
        [Url]
        public string ProfileImageUrl { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        [Required]
        public int UniversityId { get; set; }
    }
}
