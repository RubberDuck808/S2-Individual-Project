using AutoMapper;
using BLL.DTOs.Accommodation;
using Domain.Models;

public class AccommodationTypeProfile : Profile
{
    public AccommodationTypeProfile()
    {
        CreateMap<AccommodationType, AccommodationTypeDto>();
    }
}
