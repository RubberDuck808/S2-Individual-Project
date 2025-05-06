using AutoMapper;
using UniNest.DAL.Entities;
using UniNest.BLL.DTOs;

namespace UniNest.BLL.MappingProfiles
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