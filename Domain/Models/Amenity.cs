using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models
{

    public class Amenity
    {
        public int AmenityId { get; set; }


        public string Name { get; set; } = string.Empty;

        public string IconName { get; set; } = string.Empty;

    }
}