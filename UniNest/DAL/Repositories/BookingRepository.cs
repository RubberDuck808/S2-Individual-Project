using Microsoft.EntityFrameworkCore;
using UniNest.DAL.Data;
using UniNest.DAL.Entities;
using UniNest.DAL.Interfaces;

namespace UniNest.DAL.Repositories
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
