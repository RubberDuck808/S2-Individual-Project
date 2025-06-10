using AutoMapper;
using BLL.DTOs.Accommodation;
using DAL.Models;

public class AccommodationTypeProfile : Profile
{
    public AccommodationTypeProfile()
    {
        CreateMap<AccommodationType, AccommodationTypeDto>();
    }
}
