using BLL.DTOs.Shared;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IUniversityService
    {
        Task<List<UniversityDto>> GetAllAsync();
        Task<UniversityDto> GetByIdAsync(int id);
    }
}
