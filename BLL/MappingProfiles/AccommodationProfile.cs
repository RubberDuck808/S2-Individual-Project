using AutoMapper;
using DAL.Models;
using BLL.DTOs.Accommodation;

namespace BLL.MappingProfiles
{
    public class AccommodationProfile : Profile
    {
        public AccommodationProfile()
        {
            CreateMap<Accommodation, AccommodationDto>();

        }
    }
}