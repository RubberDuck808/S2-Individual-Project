using Moq;
using Xunit;
using BLL.Services;
using DAL.Interfaces;
using Domain.Models;
using Microsoft.Extensions.Logging;
using AutoMapper;
using BLL.DTOs.Booking;
using BLL.Exceptions;

namespace Tests
{
    public class BookingServiceTests
    {
        [Fact]
        public async Task GetByIdAsync_ReturnsBooking_WhenExists()
        {


            var booking = new Booking
            {
                BookingId = 9,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddMonths(6),
                TotalAmount = 1000m,
                StatusId = 1,
                StudentId = 5,
                AccommodationId = 7
            };

            var mockRepo = new Mock<IBookingRepository>();
            mockRepo.Setup(r => r.GetByIdAsync(9)).ReturnsAsync(booking);

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(m => m.Map<BookingDto>(It.IsAny<Booking>()))
                .Returns((Booking b) => new BookingDto
                {
                    BookingId = b.BookingId,
                    StartDate = b.StartDate,
                    EndDate = b.EndDate,
                    TotalAmount = b.TotalAmount,
                    StatusId = b.StatusId,
                    Status = "Mocked",
                    StudentId = b.StudentId,
                    StudentName = "Mock Student",
                    AccommodationId = b.AccommodationId,
                    AccommodationTitle = "Mock Accommodation"
                });

            var mockStatusRepo = new Mock<IStatusRepository>();
            var mockLogger = new Mock<ILogger<BookingService>>();

            var service = new BookingService(
                mockRepo.Object,
                mockMapper.Object,
                mockStatusRepo.Object,
                mockLogger.Object
            );


            var result = await service.GetByIdAsync(9);


            Assert.NotNull(result);
            Assert.Equal(9, result.BookingId);
            Assert.Equal("Mocked", result.Status);
        }

        [Fact]
        public async Task GetByIdAsync_ThrowsNotFound_WhenBookingMissing()
        {
            var mockRepo = new Mock<IBookingRepository>();
            mockRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Booking?)null);

            var mockMapper = new Mock<IMapper>();
            var mockStatusRepo = new Mock<IStatusRepository>();
            var mockLogger = new Mock<ILogger<BookingService>>();

            var service = new BookingService(
                mockRepo.Object,
                mockMapper.Object,
                mockStatusRepo.Object,
                mockLogger.Object
            );

            await Assert.ThrowsAsync<NotFoundException>(() => service.GetByIdAsync(99));
        }

        [Fact]
        public async Task UpdateStatusAsync_ThrowsForbidden_WhenStudentMismatch()
        {

            var booking = new Booking
            {
                BookingId = 9,
                StudentId = 42,
                StatusId = 1
            };

            var mockRepo = new Mock<IBookingRepository>();
            mockRepo.Setup(r => r.GetByIdAsync(9)).ReturnsAsync(booking);

            var mockMapper = new Mock<IMapper>();
            var mockStatusRepo = new Mock<IStatusRepository>();
            var mockLogger = new Mock<ILogger<BookingService>>();

            var service = new BookingService(
                mockRepo.Object,
                mockMapper.Object,
                mockStatusRepo.Object,
                mockLogger.Object
            );

            var mismatchingStudentId = 99;


            await Assert.ThrowsAsync<ForbiddenException>(() =>
                service.UpdateStatusAsync(9, "Accepted", mismatchingStudentId));
        }
    }
}
