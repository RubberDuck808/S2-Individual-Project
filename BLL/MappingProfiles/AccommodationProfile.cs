using AutoMapper;
using DAL.Entities;
using BLL.DTOs.Accommodation;

namespace BLL.MappingProfiles
{
    public class AccommodationProfile : Profile
    {
        public AccommodationProfile()
        {
            CreateMap<Accommodation, AccommodationDto>()
                .ReverseMap();
        }
    }
}