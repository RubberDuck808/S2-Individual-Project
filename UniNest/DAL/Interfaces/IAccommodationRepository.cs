using UniNest.DAL.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UniNest.DAL.Interfaces
{
    public interface IAccommodationRepository : IRepository<Accommodation>
    {
        Task<Accommodation?> GetAccommodationWithDetailsAsync(int id);
        Task<IEnumerable<Accommodation>> GetAccommodationsByLandlordAsync(int landlordId);
        Task<IEnumerable<Accommodation>> GetAccommodationsByUniversityAsync(int universityId);
        Task<IEnumerable<Accommodation>> GetAvailableAccommodationsAsync();
    }
}