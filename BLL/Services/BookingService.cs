using AutoMapper;
using BLL.DTOs.Booking;
using BLL.Exceptions;
using BLL.Interfaces;
using DAL.Interfaces;
using Domain.Models;

namespace BLL.Services
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepo;
        private readonly IMapper _mapper;
        private readonly IStatusRepository _statusRepo;

        public BookingService(IBookingRepository bookingRepo, IMapper mapper, IStatusRepository statusRepo)
        {
            _bookingRepo = bookingRepo;
            _mapper = mapper;
            _statusRepo = statusRepo;
        }

        public async Task<IEnumerable<BookingDto>> GetByStudentAsync(int studentId)
        {
            var bookings = await _bookingRepo.GetByStudentAsync(studentId);
            return _mapper.Map<IEnumerable<BookingDto>>(bookings);
        }

        public async Task<BookingDto> GetByIdAsync(int id)
        {
            var booking = await _bookingRepo.GetByIdAsync(id);
            if (booking == null)
                throw new NotFoundException($"Booking {id} not found");

            return _mapper.Map<BookingDto>(booking);
        }

        
        public async Task UpdateAsync(BookingUpdateDto dto)
        {
            var booking = await _bookingRepo.GetByIdAsync(dto.BookingId);
            if (booking == null)
                throw new NotFoundException($"Booking {dto.BookingId} not found");

            if (dto.StartDate.HasValue) booking.StartDate = dto.StartDate.Value;
            if (dto.EndDate.HasValue) booking.EndDate = dto.EndDate.Value;
            if (dto.StatusId.HasValue) booking.StatusId = dto.StatusId.Value;

            await _bookingRepo.UpdateAsync(booking);
        }

        public async Task CancelAsync(int bookingId)
        {
            var booking = await _bookingRepo.GetByIdAsync(bookingId);
            if (booking == null)
                throw new NotFoundException($"Booking {bookingId} not found");

            booking.StatusId = 3;
            await _bookingRepo.UpdateAsync(booking);
        }

        public async Task<int> CreateAsync(int studentId, int accommodationId, int applicationId)
        {
            var booking = new Booking
            {
                StudentId = studentId,
                AccommodationId = accommodationId,
                ApplicationId = applicationId,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddMonths(6),
                TotalAmount = 0, // Optional: calculate from rent
                StatusId = 1 // Active
            };

            await _bookingRepo.AddAsync(booking);
            return booking.BookingId;
        }

        public async Task UpdateStatusAsync(int bookingId, string statusName)
        {
            var booking = await _bookingRepo.GetByIdAsync(bookingId);
            if (booking == null)
                throw new NotFoundException($"Booking {bookingId} not found");

            var statusId = await _statusRepo.GetIdByNameAsync(statusName);
            if (statusId == null)
                throw new NotFoundException($"Status '{statusName}' not found");

            booking.StatusId = statusId.Value;
            await _bookingRepo.UpdateAsync(booking);
        }


    }
}
