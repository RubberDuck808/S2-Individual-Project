using UniNest.BLL.DTOs.Shared;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UniNest.BLL.Interfaces
{
    public interface IUniversityService
    {
        Task<IEnumerable<UniversityDto>> GetAllAsync();
        Task<UniversityDto> GetByIdAsync(int id);
    }
}
