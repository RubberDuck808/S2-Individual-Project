using System;
using System.ComponentModel.DataAnnotations;

namespace UniNest.BLL.DTOs.Student
{
    public class StudentCreateDto
    {
        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public int UniversityId { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [Phone]
        public string PhoneNumber { get; set; } = string.Empty;

        public string? EmergencyContact { get; set; }

        [Phone]
        public string? EmergencyPhone { get; set; }

        [Url]
        public string? ProfileImageUrl { get; set; }
    }
}
