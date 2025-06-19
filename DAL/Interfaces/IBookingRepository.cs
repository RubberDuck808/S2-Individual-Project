using Domain.Models;

namespace DAL.Interfaces
{
    public interface IBookingRepository
    {
        Task<Booking?> GetByIdAsync(int bookingId);
        Task<IEnumerable<Booking>> GetByStudentAsync(int studentId);
        Task<IEnumerable<Booking>> GetByAccommodationAsync(int accommodationId);
        Task AddAsync(Booking booking);
        Task UpdateAsync(Booking booking);
        Task DeleteAsync(int bookingId);
        //Task<IEnumerable<(Accommodation, Booking, string?)>> GetByStudentIdAsync(int studentId);

        Task<Booking?> GetAcceptedBookingByAccommodationIdAsync(int accommodationId);




    }
}
