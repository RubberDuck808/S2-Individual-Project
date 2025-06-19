using BLL.DTOs.Booking;
using Domain.Models;

namespace BLL.Interfaces
{
    public interface IBookingService
    {
        Task<IEnumerable<BookingDto>> GetByStudentAsync(int studentId);
        Task<BookingDto> GetByIdAsync(int id);
        Task UpdateAsync(BookingUpdateDto dto);
        Task CancelAsync(int bookingId);
        Task<int> CreateAsync(int studentId, int accommodationId, int applicationId);
        Task UpdateStatusAsync(int bookingId, string statusName, int? studentId = null);

        Task<string?> GetStatusNameAsync(int statusId);

        Task<Booking?> GetAcceptedBookingByAccommodationIdAsync(int accommodationId);



    }
}
