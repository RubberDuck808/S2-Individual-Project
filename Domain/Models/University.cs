using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models
{
    public class University
    {
        public int UniversityId { get; set; }

        public string Name { get; set; }
        public string City { get; set; } = string.Empty;

        public string Location { get; set; } = string.Empty;

        public double? Latitude { get; set; }
        public double? Longitude { get; set; }


    }
}