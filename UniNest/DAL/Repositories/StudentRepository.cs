using Microsoft.EntityFrameworkCore;
using UniNest.DAL.Data;
using UniNest.DAL.Entities;
using UniNest.DAL.Interfaces;

namespace UniNest.DAL.Repositories
{
    public class StudentRepository : BaseRepository<Student>, IStudentRepository
    {
        public StudentRepository(AppDbContext context) : base(context) { }

        public async Task<Student?> GetByUserIdAsync(string userId)
        {
            return await _context.Students
                .Include(s => s.University)
                .FirstOrDefaultAsync(s => s.UserId == userId);
        }

        public async Task<int> GetCountAsync()
        {
            return await _context.Students.CountAsync();
        }
    }
}
