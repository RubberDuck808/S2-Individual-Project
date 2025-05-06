using AutoMapper;
using UniNest.DAL.Entities;
using UniNest.BLL.DTOs.Landlord;

namespace UniNest.BLL.MappingProfiles
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