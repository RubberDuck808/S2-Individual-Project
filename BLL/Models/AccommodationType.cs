using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BLL.Models
{

    public class AccommodationType
    {

        public int AccommodationTypeId { get; set; }

        public string Name { get; set; } = string.Empty;

    }
}