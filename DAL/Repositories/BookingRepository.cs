using Microsoft.EntityFrameworkCore;
using DAL.Data;
using DAL.Entities;
using DAL.Interfaces;

namespace DAL.Repositories
{
    public class BookingRepository : BaseRepository<Booking>, IBookingRepository
    {
        public BookingRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<Booking>> GetByStudentAsync(int studentId)
        {
            return await _context.Bookings
                .Include(b => b.Accommodation)
                .Include(b => b.Status)
                .Where(b => b.StudentId == studentId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Booking>> GetByAccommodationAsync(int accommodationId)
        {
            return await _context.Bookings
                .Include(b => b.Student)
                .Include(b => b.Status)
                .Where(b => b.AccommodationId == accommodationId)
                .ToListAsync();
        }
    }
}
