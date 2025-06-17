using System;

namespace BLL.DTOs.Student
{
    public class StudentDto
    {
        public int StudentId { get; set; }
        public string UserId { get; set; }

        public string PhoneNumber { get; set; } 
        public string? EmergencyContact { get; set; }
        public string? EmergencyPhone { get; set; }

        public string? ProfileImageUrl { get; set; }

        public DateTime DateOfBirth { get; set; }
        public bool IsVerified { get; set; }

        public int UniversityId { get; set; }
        public string? UniversityName { get; set; }
    }
}
