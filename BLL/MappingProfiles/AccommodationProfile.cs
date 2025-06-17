using AutoMapper;
using BLL.DTOs.Accommodation;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.MappingProfiles
{
    public class AccommodationProfile : Profile
    {
        public AccommodationProfile()
        {
            CreateMap<AccommodationCreateDto, Accommodation>();
            CreateMap<AccommodationUpdateDto, Accommodation>();
            CreateMap<Accommodation, AccommodationDto>();
            CreateMap<Accommodation, LandlordAccommodationDto>()
            .ForMember(dest => dest.ApplicationCount, opt => opt.Ignore())
            .ForMember(dest => dest.UniversityName, opt => opt.Ignore())
            .ForMember(dest => dest.AmenityNames, opt => opt.Ignore())
            .ForMember(dest => dest.ImageUrls, opt => opt.Ignore())
            .ForMember(dest => dest.AccommodationType, opt => opt.Ignore());
        }
    }
}
