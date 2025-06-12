using AutoMapper;
using DAL.Models;
using BLL.DTOs.Shared;

public class StatusProfile : Profile
{
    public StatusProfile()
    {
        CreateMap<Status, StatusDto>();
        CreateMap<StatusDto, Status>();
    }
}
