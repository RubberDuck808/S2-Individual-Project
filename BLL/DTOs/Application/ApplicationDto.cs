using System;

namespace BLL.DTOs.Application
{
    public class ApplicationDto
    {
        public int ApplicationId { get; set; }

        public DateTime ApplicationDate { get; set; }
        public int StatusId { get; set; }

        public int StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;

        public int AccommodationId { get; set; }
        public string AccommodationTitle { get; set; } = string.Empty;
    }
}
