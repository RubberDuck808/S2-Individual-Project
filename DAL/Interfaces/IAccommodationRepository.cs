using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Models;


namespace DAL.Interfaces
{
    public interface IAccommodationRepository
    {
        Task<int> AddAsync(Accommodation accommodation);
        Task UpdateAsync(Accommodation accommodation);
        Task<int> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<Accommodation?> GetByIdAsync(int id);
        Task<Accommodation?> GetAccommodationWithDetailsAsync(int id);
        Task<IEnumerable<Accommodation>> GetAllAsync();
        Task<IEnumerable<Accommodation>> GetAvailableAccommodationsAsync();
        Task<IEnumerable<Accommodation>> GetFeaturedAccommodationsAsync();
        Task<IEnumerable<Accommodation>> GetAccommodationsByUniversityAsync(int universityId);
        Task<IEnumerable<Accommodation>> GetAccommodationsByLandlordAsync(int landlordId);
    }

}
