using AutoMapper;
using DAL.Entities;
using BLL.DTOs.Landlord;

namespace BLL.MappingProfiles
{
    public class LandlordProfile : Profile
    {
        public LandlordProfile()
        {
            CreateMap<Landlord, LandlordDto>()
                .ReverseMap(); 
        }
    }
}