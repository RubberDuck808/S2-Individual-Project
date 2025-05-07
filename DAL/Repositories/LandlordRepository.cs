using Microsoft.EntityFrameworkCore;
using DAL.Data;
using DAL.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.Interfaces;

namespace DAL.Repositories
{
    public class LandlordRepository : BaseRepository<Landlord>, ILandlordRepository
    {
        public LandlordRepository(AppDbContext context) : base(context) { }

        public async Task<Landlord?> GetWithPropertiesAsync(int id)
            => await _context.Landlords
                .Include(l => l.Accommodations)
                .FirstOrDefaultAsync(l => l.LandlordId == id);

        public async Task<IEnumerable<Landlord>> GetByVerificationStatusAsync(bool isVerified)
            => await _context.Landlords
                .Where(l => l.IsVerified == isVerified)
                .ToListAsync();

        public async Task<Landlord?> GetByIdAsync(int id) => await _dbSet.FindAsync(id);
        public async Task<IEnumerable<Landlord>> GetAllAsync() => await _dbSet.ToListAsync();
        public async Task AddAsync(Landlord landlord)
        {
            await _dbSet.AddAsync(landlord);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(Landlord landlord)
        {
            _dbSet.Update(landlord);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<bool> ExistsAsync(int id) => await _dbSet.FindAsync(id) != null;
        public async Task<Landlord> GetByIdWithPropertiesAsync(int id) => await _context.Landlords.Include(l => l.Accommodations).FirstOrDefaultAsync(l => l.LandlordId == id);

        public async Task<IEnumerable<Landlord>> GetByUniversityAsync(int universityId)
        {
            return await _context.Landlords
                .Where(l => l.Accommodations.Any(a => a.UniversityId == universityId))
                .ToListAsync();

        }

        public async Task<IEnumerable<Landlord>> GetActiveLandlordsAsync()
        {

            return await _context.Landlords
                .Where(l => l.IsVerified == true)
                .ToListAsync();
        }

        public async Task<Landlord?> GetByUserIdAsync(string userId)
        {
            return await _context.Landlords
                .FirstOrDefaultAsync(l => l.UserId == userId);
        }

        public async Task<int> GetCountAsync()
        {
            return await _context.Landlords.CountAsync();
        }
    }
}