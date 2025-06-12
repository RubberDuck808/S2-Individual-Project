using AutoMapper;
using DAL.Models;
using BLL.DTOs.Booking;

public class BookingProfile : Profile
{
    public BookingProfile()
    {
        CreateMap<Booking, BookingDto>();

        CreateMap<BookingUpdateDto, Booking>();
    }
}
