using UniNest.BLL.DTOs.Booking;

namespace UniNest.BLL.Interfaces
{
    public interface IBookingService
    {
        Task<IEnumerable<BookingDto>> GetByStudentAsync(int studentId);
        Task<BookingDto> GetByIdAsync(int id);
        Task<int> CreateAsync(BookingCreateDto dto);
        Task UpdateAsync(BookingUpdateDto dto);
        Task CancelAsync(int bookingId);
    }
}
