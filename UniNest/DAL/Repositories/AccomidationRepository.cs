using Microsoft.EntityFrameworkCore;
using UniNest.DAL.Data;
using UniNest.DAL.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UniNest.DAL.Interfaces;

namespace UniNest.DAL.Repositories
{
    public class AccommodationRepository : BaseRepository<Accommodation>, IAccommodationRepository
    {
        private readonly AppDbContext _context;
        private readonly DbSet<Accommodation> _dbSet; 

        public AccommodationRepository(AppDbContext context) : base(context)
        {
            _context = context;
            _dbSet = context.Set<Accommodation>(); 
        }

        public async Task<Accommodation?> GetAccommodationWithDetailsAsync(int id)
        {
            return await _context.Accommodations
                .Include(a => a.Landlord)
                .Include(a => a.AccommodationType)
                .Include(a => a.University)
                .Include(a => a.Amenities)
                    .ThenInclude(aa => aa.Amenity)
                .FirstOrDefaultAsync(a => a.AccommodationId == id);
        }

        public async Task<IEnumerable<Accommodation>> GetAccommodationsByLandlordAsync(int landlordId)
        {
            return await _context.Accommodations
                .Where(a => a.LandlordId == landlordId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Accommodation>> GetAccommodationsByUniversityAsync(int universityId)
        {
            return await _context.Accommodations
                .Where(a => a.UniversityId == universityId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Accommodation>> GetAvailableAccommodationsAsync()
        {
            return await _context.Accommodations
                .Where(a => a.IsAvailable)
                .ToListAsync();
        }
    }
}