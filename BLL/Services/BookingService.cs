using AutoMapper;
using BLL.DTOs.Booking;
using BLL.Exceptions;
using BLL.Interfaces;
using DAL.Interfaces;
using Domain.Models;
using Microsoft.Extensions.Logging;

namespace BLL.Services
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepo;
        private readonly IMapper _mapper;
        private readonly IStatusRepository _statusRepo;
        private readonly ILogger<BookingService> _logger;

        public BookingService(
            IBookingRepository bookingRepo,
            IMapper mapper,
            IStatusRepository statusRepo,
            ILogger<BookingService> logger)
        {
            _bookingRepo = bookingRepo;
            _mapper = mapper;
            _statusRepo = statusRepo;
            _logger = logger;
        }

        public async Task<IEnumerable<BookingDto>> GetByStudentAsync(int studentId)
        {
            _logger.LogInformation("Fetching bookings for student ID: {StudentId}", studentId);

            var bookings = await _bookingRepo.GetByStudentAsync(studentId);

            _logger.LogInformation("Retrieved {Count} bookings for student ID: {StudentId}", bookings.Count(), studentId);
            return _mapper.Map<IEnumerable<BookingDto>>(bookings);
        }

        public async Task<BookingDto> GetByIdAsync(int id)
        {
            _logger.LogInformation("Fetching booking with ID: {Id}", id);

            var booking = await _bookingRepo.GetByIdAsync(id);
            if (booking == null)
            {
                _logger.LogWarning("Booking with ID {Id} not found", id);
                throw new NotFoundException($"Booking {id} not found");
            }

            _logger.LogInformation("Booking with ID {Id} found", id);
            return _mapper.Map<BookingDto>(booking);
        }

        public async Task UpdateAsync(BookingUpdateDto dto)
        {
            _logger.LogInformation("Updating booking ID: {Id}", dto.BookingId);

            var booking = await _bookingRepo.GetByIdAsync(dto.BookingId);
            if (booking == null)
            {
                _logger.LogWarning("Booking with ID {Id} not found for update", dto.BookingId);
                throw new NotFoundException($"Booking {dto.BookingId} not found");
            }

            if (dto.StartDate.HasValue) booking.StartDate = dto.StartDate.Value;
            if (dto.EndDate.HasValue) booking.EndDate = dto.EndDate.Value;
            if (dto.StatusId.HasValue) booking.StatusId = dto.StatusId.Value;

            await _bookingRepo.UpdateAsync(booking);

            _logger.LogInformation("Booking ID {Id} updated successfully", dto.BookingId);
        }

        public async Task CancelAsync(int bookingId)
        {
            _logger.LogInformation("Cancelling booking ID: {Id}", bookingId);

            var booking = await _bookingRepo.GetByIdAsync(bookingId);
            if (booking == null)
            {
                _logger.LogWarning("Booking with ID {Id} not found for cancellation", bookingId);
                throw new NotFoundException($"Booking {bookingId} not found");
            }

            booking.StatusId = 3; // Cancelled
            await _bookingRepo.UpdateAsync(booking);

            _logger.LogInformation("Booking ID {Id} marked as cancelled", bookingId);
        }

        public async Task<int> CreateAsync(int studentId, int accommodationId, int applicationId)
        {
            _logger.LogInformation("Creating booking for student {StudentId}, accommodation {AccommodationId}, application {ApplicationId}",
                studentId, accommodationId, applicationId);

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

            _logger.LogInformation("Booking created with ID: {BookingId}", booking.BookingId);
            return booking.BookingId;
        }

        public async Task UpdateStatusAsync(int bookingId, string statusName)
        {
            _logger.LogInformation("Updating status of booking ID {BookingId} to '{StatusName}'", bookingId, statusName);

            var booking = await _bookingRepo.GetByIdAsync(bookingId);
            if (booking == null)
            {
                _logger.LogWarning("Booking with ID {Id} not found for status update", bookingId);
                throw new NotFoundException($"Booking {bookingId} not found");
            }

            var statusId = await _statusRepo.GetIdByNameAsync(statusName);
            if (statusId == null)
            {
                _logger.LogWarning("Status '{StatusName}' not found in status repository", statusName);
                throw new NotFoundException($"Status '{statusName}' not found");
            }

            booking.StatusId = statusId.Value;
            await _bookingRepo.UpdateAsync(booking);

            _logger.LogInformation("Booking ID {BookingId} status updated to '{StatusName}'", bookingId, statusName);
        }
    }
}
