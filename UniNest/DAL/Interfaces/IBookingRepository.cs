using UniNest.DAL.Entities;

namespace UniNest.DAL.Interfaces
{
    public interface IBookingRepository : IRepository<Booking>
    {
        Task<IEnumerable<Booking>> GetByStudentAsync(int studentId);
        Task<IEnumerable<Booking>> GetByAccommodationAsync(int accommodationId);
    }
}
