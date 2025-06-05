using AutoMapper;
using DAL.Models;
using BLL.DTOs.Accommodation;
using BLL.Models;

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