using BLL.DTOs.Accommodation;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IAccommodationAssemblerService
    {
        Task<AccommodationDto> ToDtoAsync(Accommodation entity);
    }
}
