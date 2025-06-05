using AutoMapper;
using DAL.Models;
using BLL.DTOs.Landlord;
using BLL.Models;

namespace BLL.MappingProfiles
{
    public class LandlordProfile : Profile
    {
        public LandlordProfile()
        {
            // General-purpose DTO
            CreateMap<Landlord, LandlordDto>().ReverseMap();

            // Public/basic view
            CreateMap<Landlord, LandlordBasicDto>();

            // Admin/full view
            CreateMap<Landlord, LandlordAdminDto>();
            CreateMap<LandlordUpdateDto, Landlord>();

        }
    }
}