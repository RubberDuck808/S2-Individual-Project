using Xunit;
using Moq;
using BLL.Services;
using BLL.DTOs.Accommodation;
using BLL.Exceptions;
using BLL.Interfaces;
using DAL.Interfaces;
using Domain.Models;
using Microsoft.Extensions.Logging;
using AutoMapper;

namespace Tests
{
    public class AccommodationServiceTests
    {
        [Fact]
        public async Task GetByIdAsync_ReturnsDto_WhenAccommodationExists()
        {
            // Arrange
            var accommodation = new Accommodation { AccommodationId = 5, Title = "Test Room", UniversityId = 1 };
            var dto = new AccommodationDto { AccommodationId = 5, Title = "Test Room", UniversityId = 1 };

            var mockRepo = new Mock<IAccommodationRepository>();
            mockRepo.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(accommodation);

            var mockAssembler = new Mock<IAccommodationAssemblerService>();
            mockAssembler.Setup(a => a.ToDtoAsync(accommodation)).ReturnsAsync(dto);

            var service = new AccommodationService(
                mockRepo.Object,
                Mock.Of<ILandlordRepository>(),
                Mock.Of<IAccommodationImageService>(),
                Mock.Of<IUniversityService>(),
                Mock.Of<IBookingService>(),
                Mock.Of<IMapper>(),
                Mock.Of<IApplicationService>(),
                mockAssembler.Object,
                Mock.Of<IAmenityService>(),
                Mock.Of<IAccommodationTypeService>(),
                Mock.Of<IGeoLocationService>(),
                Mock.Of<ILogger<AccommodationService>>(),
                Mock.Of<IStudentRepository>());

            // Act
            var result = await service.GetByIdAsync(5);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Test Room", result.Title);
            Assert.Equal(1, result.UniversityId);

        }
    }

}
