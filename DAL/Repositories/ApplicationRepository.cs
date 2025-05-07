using Microsoft.EntityFrameworkCore;
using DAL.Data;
using DAL.Entities;
using DAL.Interfaces;

namespace DAL.Repositories
{
    public class ApplicationRepository : BaseRepository<Application>, IApplicationRepository
    {
        public ApplicationRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<Application>> GetByStudentAsync(int studentId)
        {
            return await _context.Applications
                .Include(a => a.Accommodation)
                .Include(a => a.Status)
                .Where(a => a.StudentId == studentId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Application>> GetByLandlordIdAsync(int landlordId)
        {
            return await _context.Applications
                .Include(a => a.Accommodation)
                .ThenInclude(acc => acc.Landlord)
                .Include(a => a.Status)
                .Where(a => a.Accommodation.LandlordId == landlordId)
                .ToListAsync();
        }
    }
}
