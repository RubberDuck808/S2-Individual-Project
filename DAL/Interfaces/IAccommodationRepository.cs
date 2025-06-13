using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Models;


namespace DAL.Interfaces
{
    public interface IAccommodationRepository
    {
        Task<Accommodation?> GetAccommodationWithDetailsAsync(int id);
        Task<Accommodation?> GetByIdAsync(int id);
        Task<IEnumerable<Accommodation>> GetAllAsync();
        Task<IEnumerable<Accommodation>> GetAccommodationsByLandlordAsync(int landlordId);
        Task<IEnumerable<Accommodation>> GetAccommodationsByUniversityAsync(int universityId);
        Task<IEnumerable<Accommodation>> GetAvailableAccommodationsAsync();
        Task<IEnumerable<Accommodation>> GetFeaturedAccommodationsAsync();

        Task AddAmenitiesAsync(int accommodationId, IEnumerable<int> amenityIds);
        Task UpdateAmenitiesAsync(int accommodationId, IEnumerable<int> amenityIds);

        Task AddImagesAsync(IEnumerable<AccommodationImage> images);
        Task<int> AddAsync(Accommodation accommodation);
        Task UpdateAsync(Accommodation accommodation);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);

        Task<IEnumerable<(Accommodation accommodation, int applicationCount)>> GetWithApplicationCountsByLandlordIdAsync(int landlordId);
        Task<IEnumerable<Accommodation>> GetWithApplicationsByStudentIdAsync(int studentId);
        Task<IEnumerable<(Accommodation, Booking, string?)>> GetWithBookingsByStudentIdAsync(int studentId);
    }
}
