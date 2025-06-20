using AutoMapper;
using BLL.DTOs.Application;
using BLL.Exceptions;
using BLL.Services;
using DAL.Interfaces;
using Domain.Models;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Tests
{
    public class ApplicationServiceTests
    {
        private readonly Mock<IApplicationRepository> _mockRepo;
        private readonly Mock<ILogger<ApplicationService>> _mockLogger;
        private readonly IMapper _mapper;
        private readonly ApplicationService _service;

        public ApplicationServiceTests()
        {
            _mockRepo = new Mock<IApplicationRepository>();
            _mockLogger = new Mock<ILogger<ApplicationService>>();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Application, ApplicationDto>();
            });
            _mapper = config.CreateMapper();

            _service = new ApplicationService(_mockRepo.Object, _mapper, _mockLogger.Object);
        }

        // 1. Tests Verifies that GetByIdAsync returns a correctly mapped DTO when an application is found.
        [Fact]
        public async Task GetByIdAsync_ReturnsMappedDto_WhenFound()
        {
            var app = new Application { ApplicationId = 1, StudentId = 1, AccommodationId = 2 };
            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(app);

            var result = await _service.GetByIdAsync(1);

            Assert.Equal(app.ApplicationId, result.ApplicationId);
            _mockRepo.Verify(r => r.GetByIdAsync(1), Times.Once);
        }

        // 2. Tests - Edge Case - Verifies that GetByIdAsync throws a NotFoundException if the application does not exist.
        [Fact]
        public async Task GetByIdAsync_ThrowsNotFoundException_WhenNull()
        {
            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Application)null);

            await Assert.ThrowsAsync<NotFoundException>(() => _service.GetByIdAsync(1));
            _mockRepo.Verify(r => r.GetByIdAsync(1), Times.Once);
        }

        // 3. Tests Verifies that CreateAsync successfully creates an application and returns its new ID.
        [Fact]
        public async Task CreateAsync_ReturnsNewId_WhenValid()
        {
            var dto = new ApplicationCreateDto { StudentId = 1, AccommodationId = 2 };
            _mockRepo.Setup(r => r.ExistsAsync(dto.StudentId, dto.AccommodationId)).ReturnsAsync(false);
            _mockRepo.Setup(r => r.CreateAsync(It.IsAny<Application>())).ReturnsAsync(10);

            var result = await _service.CreateAsync(dto);

            Assert.Equal(10, result);
            _mockRepo.Verify(r => r.CreateAsync(It.IsAny<Application>()), Times.Once);
        }

        // 4. Tests - Edge Case - Verifies that CreateAsync throws a ValidationException if a student has already applied to the same accommodation.
        [Fact]
        public async Task CreateAsync_ThrowsValidation_WhenAlreadyExists()
        {
            var dto = new ApplicationCreateDto { StudentId = 1, AccommodationId = 2 };
            _mockRepo.Setup(r => r.ExistsAsync(dto.StudentId, dto.AccommodationId)).ReturnsAsync(true);

            await Assert.ThrowsAsync<ValidationException>(() => _service.CreateAsync(dto));
        }

        // 5. Tests Verifies that UpdateStatusAsync correctly changes the status of an existing application.
        [Fact]
        public async Task UpdateStatusAsync_UpdatesStatus_WhenFound()
        {
            var dto = new ApplicationUpdateDto { ApplicationId = 1, StatusId = 2 };
            var existingApp = new Application { ApplicationId = 1, StatusId = 1 };
            _mockRepo.Setup(r => r.GetByIdAsync(dto.ApplicationId)).ReturnsAsync(existingApp);

            await _service.UpdateStatusAsync(dto);

            Assert.Equal(2, existingApp.StatusId);
            _mockRepo.Verify(r => r.UpdateAsync(existingApp), Times.Once);
        }

        // 6. Tests - Edge Case - Verifies that UpdateStatusAsync throws a NotFoundException when trying to update a non-existent application.
        [Fact]
        public async Task UpdateStatusAsync_ThrowsNotFound_WhenMissing()
        {
            var dto = new ApplicationUpdateDto { ApplicationId = 1, StatusId = 2 };
            _mockRepo.Setup(r => r.GetByIdAsync(dto.ApplicationId)).ReturnsAsync((Application)null);

            await Assert.ThrowsAsync<NotFoundException>(() => _service.UpdateStatusAsync(dto));
        }

        // 7. Tests Verifies that ExistsAsync returns true when an application record exists.
        [Fact]
        public async Task ExistsAsync_ReturnsTrue_WhenExists()
        {
            _mockRepo.Setup(r => r.ExistsAsync(1, 2)).ReturnsAsync(true);

            var result = await _service.ExistsAsync(1, 2);

            Assert.True(result);
        }

        // 8. Tests Verifies that SelectApplicantAsync correctly calls the repository methods to mark one applicant as selected and reject others.
        [Fact]
        public async Task SelectApplicantAsync_CallsRepoMethods()
        {
            await _service.SelectApplicantAsync(3, 4);

            _mockRepo.Verify(r => r.MarkAsSelectedAsync(3), Times.Once);
            _mockRepo.Verify(r => r.RejectOthersAsync(4, 3), Times.Once);
        }
    }
}
