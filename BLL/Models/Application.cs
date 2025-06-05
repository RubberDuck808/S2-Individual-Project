using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BLL.Models
{
    public class Application
    {
        public int ApplicationId { get; set; }

        public int StudentId { get; set; }

        public int AccommodationId { get; set; }

        public int StatusId { get; set; }

        public DateTime ApplicationDate { get; set; } = DateTime.UtcNow;
    }

}
