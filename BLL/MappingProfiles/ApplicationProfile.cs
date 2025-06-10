using AutoMapper;
using BLL.DTOs.Application;
using DAL.Models;

namespace BLL.MappingProfiles
{
    public class ApplicationProfile : Profile
    {
        public ApplicationProfile()
        {
            // Entity → DTO
            CreateMap<Application, ApplicationDto>();

            // DTO → Entity (for creation)
            CreateMap<ApplicationCreateDto, Application>();

            // DTO → Entity (for status updates)
            CreateMap<ApplicationUpdateDto, Application>();
        }
    }
}
