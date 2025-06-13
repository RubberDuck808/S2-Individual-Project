using AutoMapper;
using BLL.DTOs.Shared;
using Domain.Models;

public class UniversityProfile : Profile
{
    public UniversityProfile()
    {
        CreateMap<University, UniversityDto>();
    }
}
