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
        }
    }
}
