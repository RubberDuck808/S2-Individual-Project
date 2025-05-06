using System;

namespace UniNest.BLL.DTOs.Application
{
    public class ApplicationDto
    {
        public int ApplicationId { get; set; }

        public DateTime ApplicationDate { get; set; }
        public string Status { get; set; } = string.Empty;

        public int StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;

        public int AccommodationId { get; set; }
        public string AccommodationTitle { get; set; } = string.Empty;
    }
}
