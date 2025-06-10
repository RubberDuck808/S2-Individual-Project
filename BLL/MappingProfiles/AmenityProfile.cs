using AutoMapper;
using BLL.DTOs.Shared;
using DAL.Models;

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
