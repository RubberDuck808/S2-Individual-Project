using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Models
{
    public class Status
    {
        public int StatusId { get; set; }

        public string Name { get; set; } = string.Empty;
    }
}