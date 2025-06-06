using AutoMapper;
using DAL.Models;
using BLL.DTOs.Accommodation;

namespace BLL.MappingProfiles
{
    public class AccommodationProfile : Profile
    {
        public AccommodationProfile()
        {
            // Entity → DTO
            CreateMap<Accommodation, AccommodationDto>();

            // DTO → Entity (for creation)
            CreateMap<AccommodationCreateDto, Accommodation>()
                .ForMember(dest => dest.IsAvailable, opt => opt.MapFrom(src => true));

            // DTO → Entity (for update)
            CreateMap<AccommodationUpdateDto, Accommodation>();
        }
    }
}
