using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IAccommodationImageService
    {
        Task<List<AccommodationImage>> GetByAccommodationIdAsync(int accommodationId);
        Task AddImagesAsync(IEnumerable<AccommodationImage> images);
    }

}
