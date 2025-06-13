using AutoMapper;
using Domain.Models;
using BLL.DTOs.Landlord;


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