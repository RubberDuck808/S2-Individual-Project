using AutoMapper;
using BLL.DTOs.Shared;
using Domain.Models;

namespace BLL.MappingProfiles
{
    public class AmenityProfile : Profile
    {
        public AmenityProfile()
        {
            CreateMap<Amenity, AmenityDto>();
        }
    }

}
