using AutoMapper;
using Domain.Models;
using BLL.DTOs.Booking;

public class BookingProfile : Profile
{
    public BookingProfile()
    {
        CreateMap<Booking, BookingDto>();

        CreateMap<BookingUpdateDto, Booking>();
    }
}
