using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Models
{
    public class University
    {
        public int UniversityId { get; set; }

        public string Name { get; set; }

        public string Location { get; set; } = string.Empty;
    }
}