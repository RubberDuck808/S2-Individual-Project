using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static System.Net.Mime.MediaTypeNames;

namespace DAL.Models
{
    public class Student
    {
        public int StudentId { get; set; }

        public string UserId { get; set; }

        public int UniversityId { get; set; }

        public string Email { get; set; } = string.Empty;

        public string FirstName { get; set; }

        public string? MiddleName { get; set; }

        public string LastName { get; set; }

        public DateTime DateOfBirth { get; set; }

        public string PhoneNumber { get; set; }

        public string EmergencyContact { get; set; }

        public string EmergencyPhone { get; set; }

        public string ProfileImageUrl { get; set; }

        public bool IsVerified { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Constructor for required fields
        public Student(string userId, int universityId, DateTime dateOfBirth, string phoneNumber)
        {
            UserId = userId ?? throw new ArgumentNullException(nameof(userId));
            UniversityId = universityId;
            DateOfBirth = dateOfBirth;
            PhoneNumber = phoneNumber ?? throw new ArgumentNullException(nameof(phoneNumber));
        }

        public Student() { }
    }
}