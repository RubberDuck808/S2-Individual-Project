using BLL.DTOs.Booking;

namespace BLL.Interfaces
{
    public interface IBookingService
    {
        Task<IEnumerable<BookingDto>> GetByStudentAsync(int studentId);
        Task<BookingDto> GetByIdAsync(int id);
        Task UpdateAsync(BookingUpdateDto dto);
        Task CancelAsync(int bookingId);
        Task<int> CreateAsync(int studentId, int accommodationId, int applicationId);
        Task UpdateStatusAsync(int bookingId, string statusName);

        Task<string?> GetStatusNameAsync(int statusId);


    }
}
