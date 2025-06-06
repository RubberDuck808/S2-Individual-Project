using DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL.Interfaces
{
    public interface IAccommodationRepository : IRepository<Accommodation>
    {
        Task<Accommodation?> GetAccommodationWithDetailsAsync(int id);
        Task<IEnumerable<Accommodation>> GetAccommodationsByLandlordAsync(int landlordId);
        Task<IEnumerable<Accommodation>> GetAccommodationsByUniversityAsync(int universityId);
        Task<IEnumerable<Accommodation>> GetAvailableAccommodationsAsync();
        Task AddAmenitiesAsync(int accommodationId, IEnumerable<int> amenityIds);
        Task AddImagesAsync(IEnumerable<AccommodationImage> images);

        Task<int> AddAsync(Accommodation accommodation);



    }
}