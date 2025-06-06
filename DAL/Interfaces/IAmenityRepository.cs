using DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL.Interfaces
{
    public interface IAmenityRepository
    {
        Task<List<Amenity>> GetAllAsync();
        Task<List<Amenity>> GetByAccommodationIdAsync(int accommodationId);
    }
}
