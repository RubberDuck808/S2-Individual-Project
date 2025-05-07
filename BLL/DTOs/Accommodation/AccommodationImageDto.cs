using System;

namespace BLL.DTOs.Accommodation
{
    public class AccommodationImageDto
    {
        public int ImageId { get; set; }
        public int AccommodationId { get; set; }
        public string Url { get; set; } = string.Empty;
        public bool IsPrimary { get; set; }
        public DateTime UploadedAt { get; set; }
    }
}
