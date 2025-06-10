using AutoMapper;
using BLL.DTOs.Shared;
using DAL.Models;

public class UniversityProfile : Profile
{
    public UniversityProfile()
    {
        CreateMap<University, UniversityDto>();
    }
}
