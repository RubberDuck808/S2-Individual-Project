using AutoMapper;
using BLL.DTOs.Landlord;
using BLL.Exceptions;
using BLL.Services;
using DAL.Interfaces;
using Domain.Models;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Tests
{
    public class LandlordServiceTests
    {
        // 1. Tests if a landlord is correctly fetched and mapped to DTO when found.
        [Fact]
        public async Task GetLandlordAsync_ReturnsDto_WhenLandlordExists()
        {
            // Arrange
            int landlordId = 1;
            var landlord = new Landlord
            {
                LandlordId = landlordId,
                FirstName = "John",
                Accommodations = new List<Accommodation>
                {
                    new Accommodation { IsAvailable = true, MonthlyRent = 500 },
                    new Accommodation { IsAvailable = false, MonthlyRent = 700 }
                }
            };

            var expectedDto = new LandlordDto
            {
                LandlordId = landlordId,
                FirstName = "John",
                ActiveListingsCount = 1,
                TotalMonthlyRent = 1200
            };

            var mockRepo = new Mock<ILandlordRepository>();
            mockRepo.Setup(r => r.GetByIdWithPropertiesAsync(landlordId)).ReturnsAsync(landlord);

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(m => m.Map<LandlordDto>(It.Is<Landlord>(l => l.LandlordId == landlordId))).Returns(expectedDto);

            var service = new LandlordService(mockRepo.Object, Mock.Of<IAccommodationRepository>(), mockMapper.Object, Mock.Of<ILogger<LandlordService>>());

            // Act
            var result = await service.GetLandlordAsync(landlordId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedDto.LandlordId, result.LandlordId);
            Assert.Equal(expectedDto.FirstName, result.FirstName);
            Assert.Equal(expectedDto.ActiveListingsCount, result.ActiveListingsCount);
            Assert.Equal(expectedDto.TotalMonthlyRent, result.TotalMonthlyRent);

            // Verify interactions
            mockRepo.Verify(r => r.GetByIdWithPropertiesAsync(landlordId), Times.Once);
            mockMapper.Verify(m => m.Map<LandlordDto>(landlord), Times.Once);
        }



        // 2. Tests that NotFoundException is thrown when landlord is missing.
        [Fact]
        public async Task GetLandlordAsync_ThrowsNotFound_WhenLandlordDoesNotExist()
        {
            // Arrange
            int nonExistentLandlordId = 99;
            var mockRepo = new Mock<ILandlordRepository>();
            mockRepo.Setup(r => r.GetByIdWithPropertiesAsync(nonExistentLandlordId)).ReturnsAsync((Landlord)null);

            var mockMapper = new Mock<IMapper>();
            var service = new LandlordService(mockRepo.Object, Mock.Of<IAccommodationRepository>(), mockMapper.Object, Mock.Of<ILogger<LandlordService>>());

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => service.GetLandlordAsync(nonExistentLandlordId));
            mockRepo.Verify(r => r.GetByIdWithPropertiesAsync(nonExistentLandlordId), Times.Once);
            mockMapper.Verify(m => m.Map<LandlordDto>(It.IsAny<Landlord>()), Times.Never); 
        }


        // 3. Tests profile update correctly updates landlord fields and calls update.
        [Fact]
        public async Task UpdateLandlordProfileAsync_UpdatesFields_WhenLandlordExists()
        {
            // Arrange
            int landlordId = 1;
            var landlord = new Landlord
            {
                LandlordId = landlordId,
                FirstName = "Old",
                Email = "old@example.com",
                LastName = "Doe",
                PhoneNumber = "111",
                CompanyName = "OldCo",
                TaxIdentificationNumber = "OLDTAX"
            };

            var updateDto = new LandlordUpdateDto
            {
                FirstName = "New",
                Email = "new@example.com",
                PhoneNumber = "222"
            };

            var mockRepo = new Mock<ILandlordRepository>();
            mockRepo.Setup(r => r.GetByIdAsync(landlordId)).ReturnsAsync(landlord);
            mockRepo.Setup(r => r.UpdateAsync(It.IsAny<Landlord>())).Returns(Task.CompletedTask);

            var service = new LandlordService(mockRepo.Object, Mock.Of<IAccommodationRepository>(), Mock.Of<IMapper>(), Mock.Of<ILogger<LandlordService>>());

            // Act
            await service.UpdateLandlordProfileAsync(landlordId, updateDto);

            // Assert
            Assert.Equal("New", landlord.FirstName);
            Assert.Equal("new@example.com", landlord.Email);
            Assert.Equal("222", landlord.PhoneNumber);
            Assert.Equal("Doe", landlord.LastName);
            Assert.Equal("OldCo", landlord.CompanyName);

            mockRepo.Verify(r => r.GetByIdAsync(landlordId), Times.Once);
            mockRepo.Verify(r => r.UpdateAsync(It.Is<Landlord>(l => l.LandlordId == landlordId && l.FirstName == "New")), Times.Once);
        }



        // 4. Tests NotFoundException is thrown when updating a non-existing landlord.
        [Fact]
        public async Task UpdateLandlordProfileAsync_ThrowsNotFound_WhenLandlordDoesNotExist()
        {
            // Arrange
            int nonExistentLandlordId = 1;
            var mockRepo = new Mock<ILandlordRepository>();
            mockRepo.Setup(r => r.GetByIdAsync(nonExistentLandlordId)).ReturnsAsync((Landlord)null);

            var service = new LandlordService(mockRepo.Object, Mock.Of<IAccommodationRepository>(), Mock.Of<IMapper>(), Mock.Of<ILogger<LandlordService>>());

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => service.UpdateLandlordProfileAsync(nonExistentLandlordId, new LandlordUpdateDto()));
            mockRepo.Verify(r => r.GetByIdAsync(nonExistentLandlordId), Times.Once);
            mockRepo.Verify(r => r.UpdateAsync(It.IsAny<Landlord>()), Times.Never);
        }



        // 5. Tests that landlord is returned and calculated fields are included when found by user ID.
        [Fact]
        public async Task GetByUserIdAsync_ReturnsDto_WhenLandlordExistsForUserId()
        {
            // Arrange
            string userId = "user123";
            var landlord = new Landlord
            {
                UserId = userId,
                LandlordId = 5,
                Accommodations = new List<Accommodation>
                {
                    new Accommodation { IsAvailable = true, MonthlyRent = 500 },
                    new Accommodation { IsAvailable = false, MonthlyRent = 600 }
                }
            };

            var expectedDto = new LandlordDto
            {
                LandlordId = 5,
                ActiveListingsCount = 1,
                TotalMonthlyRent = 1100
            };

            var mockRepo = new Mock<ILandlordRepository>();
            mockRepo.Setup(r => r.GetByUserIdAsync(userId)).ReturnsAsync(landlord);

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(m => m.Map<LandlordDto>(It.Is<Landlord>(l => l.UserId == userId))).Returns(expectedDto);

            var service = new LandlordService(mockRepo.Object, Mock.Of<IAccommodationRepository>(), mockMapper.Object, Mock.Of<ILogger<LandlordService>>());

            // Act
            var result = await service.GetByUserIdAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedDto.LandlordId, result.LandlordId);
            Assert.Equal(expectedDto.ActiveListingsCount, result.ActiveListingsCount);
            Assert.Equal(expectedDto.TotalMonthlyRent, result.TotalMonthlyRent);

            mockRepo.Verify(r => r.GetByUserIdAsync(userId), Times.Once);
            mockMapper.Verify(m => m.Map<LandlordDto>(landlord), Times.Once);
        }



        // 6. Tests that null is returned when landlord is not found by user ID.
        [Fact]
        public async Task GetByUserIdAsync_ReturnsNull_WhenLandlordDoesNotExistForUserId()
        {
            // Arrange
            string nonExistentUserId = "user999";
            var mockRepo = new Mock<ILandlordRepository>();
            mockRepo.Setup(r => r.GetByUserIdAsync(nonExistentUserId)).ReturnsAsync((Landlord)null);

            var mockMapper = new Mock<IMapper>(); 
            var service = new LandlordService(mockRepo.Object, Mock.Of<IAccommodationRepository>(), mockMapper.Object, Mock.Of<ILogger<LandlordService>>());

            // Act
            var result = await service.GetByUserIdAsync(nonExistentUserId);

            // Assert
            Assert.Null(result);
            mockRepo.Verify(r => r.GetByUserIdAsync(nonExistentUserId), Times.Once);
            mockMapper.Verify(m => m.Map<LandlordDto>(It.IsAny<Landlord>()), Times.Never); 
        }


        // 7. Tests fetching all verified landlords and mapping to DTOs.
        [Fact]
        public async Task GetAllVerifiedLandlordsAsync_ReturnsAllVerifiedLandlords()
        {
            // Arrange
            var landlords = new List<Landlord>
            {
                new Landlord { LandlordId = 11, IsVerified = true },
                new Landlord { LandlordId = 12, IsVerified = true }
            };
            var expectedDtos = new List<LandlordDto>
            {
                new LandlordDto { LandlordId = 11, FirstName = "Verified A" },
                new LandlordDto { LandlordId = 12, FirstName = "Verified B" }
            };

            var mockRepo = new Mock<ILandlordRepository>();
            mockRepo.Setup(r => r.GetActiveLandlordsAsync()).ReturnsAsync(landlords);

            var mockMapper = new Mock<IMapper>(); 
            mockMapper.Setup(m => m.Map<IEnumerable<LandlordDto>>(It.IsAny<IEnumerable<Landlord>>())).Returns(expectedDtos);

            var service = new LandlordService(mockRepo.Object, Mock.Of<IAccommodationRepository>(), mockMapper.Object, Mock.Of<ILogger<LandlordService>>());

            // Act
            var result = (await service.GetAllVerifiedLandlordsAsync()).ToList();

            // Assert
            Assert.Equal(expectedDtos.Count, result.Count);
            Assert.Contains(result, l => l.LandlordId == 11);
            Assert.Contains(result, l => l.LandlordId == 12);

            mockRepo.Verify(r => r.GetActiveLandlordsAsync(), Times.Once);
            mockMapper.Verify(m => m.Map<IEnumerable<LandlordDto>>(landlords), Times.Once);
        }

        // 8. Tests - Edge Case - No verified landlords
        [Fact]
        public async Task GetAllVerifiedLandlordsAsync_ReturnsEmptyList_WhenNoVerifiedLandlords()
        {
            // Arrange
            var mockRepo = new Mock<ILandlordRepository>();
            mockRepo.Setup(r => r.GetActiveLandlordsAsync()).ReturnsAsync(new List<Landlord>());

            var mockMapper = new Mock<IMapper>(); 
            mockMapper.Setup(m => m.Map<IEnumerable<LandlordDto>>(It.IsAny<IEnumerable<Landlord>>())).Returns(new List<LandlordDto>());

            var service = new LandlordService(mockRepo.Object, Mock.Of<IAccommodationRepository>(), mockMapper.Object, Mock.Of<ILogger<LandlordService>>());

            // Act
            var result = (await service.GetAllVerifiedLandlordsAsync()).ToList();

            // Assert
            Assert.Empty(result);
            mockRepo.Verify(r => r.GetActiveLandlordsAsync(), Times.Once);
            mockMapper.Verify(m => m.Map<IEnumerable<LandlordDto>>(It.IsAny<IEnumerable<Landlord>>()), Times.Once);
        }



        // 9. Tests Fetching and mapping of all landlords regardless of verification.
        [Fact]
        public async Task GetAllAsync_ReturnsAllLandlords()
        {
            // Arrange
            var landlords = new List<Landlord>
            {
                new Landlord { LandlordId = 100 },
                new Landlord { LandlordId = 101 }
            };
            var expectedDtos = new List<LandlordDto>
            {
                new LandlordDto { LandlordId = 100, FirstName = "Landlord C" },
                new LandlordDto { LandlordId = 101, FirstName = "Landlord D" }
            };

            var mockRepo = new Mock<ILandlordRepository>();
            mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(landlords);

            var mockMapper = new Mock<IMapper>(); 
            mockMapper.Setup(m => m.Map<IEnumerable<LandlordDto>>(It.IsAny<IEnumerable<Landlord>>())).Returns(expectedDtos);

            var service = new LandlordService(mockRepo.Object, Mock.Of<IAccommodationRepository>(), mockMapper.Object, Mock.Of<ILogger<LandlordService>>());

            // Act
            var result = (await service.GetAllAsync()).ToList();

            // Assert
            Assert.Equal(expectedDtos.Count, result.Count);
            Assert.Contains(result, l => l.LandlordId == 100);
            Assert.Contains(result, l => l.LandlordId == 101);

            mockRepo.Verify(r => r.GetAllAsync(), Times.Once);
            mockMapper.Verify(m => m.Map<IEnumerable<LandlordDto>>(landlords), Times.Once);
        }

        // 10. Tests - Edge Case - No landlords at all
        [Fact]
        public async Task GetAllAsync_ReturnsEmptyList_WhenNoLandlordsExist()
        {
            // Arrange
            var mockRepo = new Mock<ILandlordRepository>();
            mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Landlord>());

            var mockMapper = new Mock<IMapper>(); 
            mockMapper.Setup(m => m.Map<IEnumerable<LandlordDto>>(It.IsAny<IEnumerable<Landlord>>())).Returns(new List<LandlordDto>());

            var service = new LandlordService(mockRepo.Object, Mock.Of<IAccommodationRepository>(), mockMapper.Object, Mock.Of<ILogger<LandlordService>>());

            // Act
            var result = (await service.GetAllAsync()).ToList();

            // Assert
            Assert.Empty(result);
            mockRepo.Verify(r => r.GetAllAsync(), Times.Once);
            mockMapper.Verify(m => m.Map<IEnumerable<LandlordDto>>(It.IsAny<IEnumerable<Landlord>>()), Times.Once);
        }
    }
}