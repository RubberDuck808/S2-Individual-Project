using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models
{

    public class Booking
    {
        public int BookingId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public decimal TotalAmount { get; set; }

        // Foreign Keys
        public int StudentId { get; set; }

        public int AccommodationId { get; set; }

        public int StatusId { get; set; }

        public int ApplicationId { get; set; }

    }

}