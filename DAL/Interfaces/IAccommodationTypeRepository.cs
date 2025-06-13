using Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL.Interfaces
{
    public interface IAccommodationTypeRepository
    {
        Task<List<AccommodationType>> GetAllAsync();
        Task<AccommodationType?> GetByIdAsync(int id);

    }
}
