using System.ComponentModel.DataAnnotations;

namespace UniNest.BLL.DTOs.Student
{
    public class StudentUpdateDto
    {
        [Phone]
        public string? PhoneNumber { get; set; }

        public string? EmergencyContact { get; set; }

        [Phone]
        public string? EmergencyPhone { get; set; }

        [Url]
        public string? ProfileImageUrl { get; set; }
    }
}
