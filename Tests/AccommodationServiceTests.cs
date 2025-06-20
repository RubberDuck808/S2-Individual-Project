using BLL.DTOs.Accommodation;
using BLL.DTOs.Application;
using BLL.DTOs.Booking;
using BLL.Exceptions;
using BLL.Interfaces;
using BLL.Services;
using DAL.Interfaces;
using Domain.Models;
using Microsoft.Extensions.Logging;
using AutoMapper;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using Xunit;
using Moq;

namespace Tests
{
    public static class ErrorMessages
    {
        public const string AccommodationNotFound = "Accommodation with ID {0} not found.";
        public const string ApplicationNotFound = "Application with ID {0} not found.";
        public const string BookingNotFound = "Booking with ID {0} not found.";
        public const string StatusNotFound = "Status '{0}' not found.";
        public const string UnauthorizedBookingModification = "Unauthorized booking modification.";
    }

    public class AccommodationServiceTests
    {
        private AccommodationService CreateService(
            IAccommodationRepository accommodationRepo = null,
            ILandlordRepository landlordRepo = null,
            IAccommodationImageService imageService = null,
            IUniversityService universityService = null,
            IBookingService bookingService = null,
            IMapper mapper = null,
            IApplicationService applicationService = null,
            IAccommodationAssemblerService assemblerService = null,
            IAmenityService amenityService = null,
            IAccommodationTypeService typeService = null,
            IGeoLocationService geoLocationService = null,
            ILogger<AccommodationService> logger = null,
            IStudentRepository studentRepo = null)
        {
            return new AccommodationService(
                accommodationRepo ?? Mock.Of<IAccommodationRepository>(),
                landlordRepo ?? Mock.Of<ILandlordRepository>(),
                imageService ?? Mock.Of<IAccommodationImageService>(),
                universityService ?? Mock.Of<IUniversityService>(),
                bookingService ?? Mock.Of<IBookingService>(),
                mapper ?? Mock.Of<IMapper>(),
                applicationService ?? Mock.Of<IApplicationService>(),
                assemblerService ?? Mock.Of<IAccommodationAssemblerService>(),
                amenityService ?? Mock.Of<IAmenityService>(),
                typeService ?? Mock.Of<IAccommodationTypeService>(),
                geoLocationService ?? Mock.Of<IGeoLocationService>(),
                logger ?? Mock.Of<ILogger<AccommodationService>>(),
                studentRepo ?? Mock.Of<IStudentRepository>()
            );
        }

        // 1. Tests that GetByIdAsync returns the correct DTO when an accommodation exists.
        [Fact]
        public async Task GetByIdAsync_ReturnsAccommodationDto_WhenAccommodationExists()
        {
            var accommodationId = 1;
            var mockAccommodation = new Accommodation { AccommodationId = accommodationId, Title = "Test Acc", UniversityId = 1, AccommodationTypeId = 1 };
            var expectedDto = new AccommodationDto { AccommodationId = accommodationId, Title = "Test Acc", UniversityName = "Uni A", AccommodationType = "Type B" };

            var mockAccommodationRepo = new Mock<IAccommodationRepository>();
            mockAccommodationRepo.Setup(r => r.GetByIdAsync(accommodationId)).ReturnsAsync(mockAccommodation);

            var mockAssembler = new Mock<IAccommodationAssemblerService>();
            mockAssembler.Setup(a => a.ToDtoAsync(mockAccommodation)).ReturnsAsync(expectedDto);

            var service = CreateService(
                accommodationRepo: mockAccommodationRepo.Object,
                assemblerService: mockAssembler.Object);

            var result = await service.GetByIdAsync(accommodationId);

            Assert.NotNull(result);
            Assert.Equal(accommodationId, result.AccommodationId);
            Assert.Equal(expectedDto.Title, result.Title);
            mockAccommodationRepo.Verify(r => r.GetByIdAsync(accommodationId), Times.Once);
            mockAssembler.Verify(a => a.ToDtoAsync(mockAccommodation), Times.Once);
        }

        // 2. Tests that UpdateAsync correctly updates an existing accommodation's properties.
        [Fact]
        public async Task UpdateAsync_UpdatesAccommodation_WhenAccommodationExists()
        {
            var accommodationId = 1;
            var updateDto = new AccommodationUpdateDto { AccommodationId = accommodationId, Title = "Updated Title", MonthlyRent = 1200, UniversityId = 2, AccommodationTypeId = 3 };
            var existingAccommodation = new Accommodation { AccommodationId = accommodationId, Title = "Original Title", MonthlyRent = 1000, UniversityId = 1, AccommodationTypeId = 1 };

            var mockAccommodationRepo = new Mock<IAccommodationRepository>();
            mockAccommodationRepo.Setup(r => r.GetByIdAsync(accommodationId)).ReturnsAsync(existingAccommodation);
            mockAccommodationRepo.Setup(r => r.UpdateAsync(It.IsAny<Accommodation>())).Returns(Task.CompletedTask);

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(m => m.Map(updateDto, existingAccommodation))
                      .Callback<object, object>((src, dest) =>
                      {
                          var sourceDto = (AccommodationUpdateDto)src;
                          var destEntity = (Accommodation)dest;
                          destEntity.Title = sourceDto.Title;
                          destEntity.MonthlyRent = sourceDto.MonthlyRent;
                          destEntity.UniversityId = sourceDto.UniversityId;
                          destEntity.AccommodationTypeId = sourceDto.AccommodationTypeId;
                      });

            var service = CreateService(
                accommodationRepo: mockAccommodationRepo.Object,
                mapper: mockMapper.Object);

            await service.UpdateAsync(updateDto);

            mockAccommodationRepo.Verify(r => r.GetByIdAsync(accommodationId), Times.Once);
            mockMapper.Verify(m => m.Map(updateDto, existingAccommodation), Times.Once);
            mockAccommodationRepo.Verify(r => r.UpdateAsync(
                It.Is<Accommodation>(acc => acc.AccommodationId == accommodationId &&
                                            acc.Title == updateDto.Title &&
                                            acc.MonthlyRent == updateDto.MonthlyRent &&
                                            acc.UniversityId == updateDto.UniversityId &&
                                            acc.AccommodationTypeId == updateDto.AccommodationTypeId)), Times.Once);
        }

        // 3. Tests that DeleteAsync successfully removes an existing accommodation.
        [Fact]
        public async Task DeleteAsync_DeletesAccommodation_WhenAccommodationExists()
        {
            var accommodationId = 1;
            var mockAccommodationRepo = new Mock<IAccommodationRepository>();
            mockAccommodationRepo.Setup(r => r.DeleteAsync(accommodationId)).ReturnsAsync(1);

            var service = CreateService(accommodationRepo: mockAccommodationRepo.Object);

            await service.DeleteAsync(accommodationId);

            mockAccommodationRepo.Verify(r => r.DeleteAsync(accommodationId), Times.Once);
        }

        // 4. Tests that CreateAsync successfully creates a new accommodation and its associated amenities.
        [Fact]
        public async Task CreateAsync_CreatesAccommodationAndAddsAmenities()
        {
            var createDto = new AccommodationCreateDto
            {
                Title = "New Acc",
                LandlordId = 1,
                UniversityId = 1,
                AccommodationTypeId = 1,
                Address = "123 Main St",
                City = "Testville",
                PostCode = "12345",
                Country = "Testland"
            };
            var amenityIds = new List<int> { 101, 102 };
            var newAccommodationId = 5;
            var mappedAccommodation = new Accommodation { Title = createDto.Title, LandlordId = createDto.LandlordId };

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(m => m.Map<Accommodation>(createDto)).Returns(mappedAccommodation);

            var mockAccommodationRepo = new Mock<IAccommodationRepository>();
            mockAccommodationRepo.Setup(r => r.AddAsync(mappedAccommodation)).ReturnsAsync(newAccommodationId);

            var mockAmenityService = new Mock<IAmenityService>();
            mockAmenityService.Setup(s => s.AddAsync(newAccommodationId, amenityIds)).Returns(Task.CompletedTask);

            var mockGeoLocationService = new Mock<IGeoLocationService>();
            mockGeoLocationService.Setup(g => g.GetCoordinatesFromAddressAsync(It.IsAny<string>())).ReturnsAsync((52.0, 5.0));

            var service = CreateService(
                accommodationRepo: mockAccommodationRepo.Object,
                mapper: mockMapper.Object,
                amenityService: mockAmenityService.Object,
                geoLocationService: mockGeoLocationService.Object);

            var resultId = await service.CreateAsync(createDto, amenityIds);

            Assert.Equal(newAccommodationId, resultId);
            mockMapper.Verify(m => m.Map<Accommodation>(createDto), Times.Once);
            mockGeoLocationService.Verify(g => g.GetCoordinatesFromAddressAsync(It.IsAny<string>()), Times.Once);
            mockAccommodationRepo.Verify(r => r.AddAsync(mappedAccommodation), Times.Once);
            mockAmenityService.Verify(s => s.AddAsync(newAccommodationId, amenityIds), Times.Once);
        }

        // 5. Tests that UpdateWithAmenitiesAsync correctly updates both the accommodation details and its associated amenities.
        [Fact]
        public async Task UpdateWithAmenitiesAsync_UpdatesAccommodationAndAmenities()
        {
            var accommodationId = 1;
            var updateDto = new AccommodationUpdateDto { AccommodationId = accommodationId, Title = "Updated Title", MonthlyRent = 1200 };
            var amenityIds = new List<int> { 201, 202, 203 };
            var mappedAccommodationEntity = new Accommodation { AccommodationId = accommodationId, Title = updateDto.Title, MonthlyRent = updateDto.MonthlyRent };

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(m => m.Map<Accommodation>(updateDto)).Returns(mappedAccommodationEntity);

            var mockAccommodationRepo = new Mock<IAccommodationRepository>();
            mockAccommodationRepo.Setup(r => r.UpdateAsync(It.IsAny<Accommodation>())).Returns(Task.CompletedTask);

            var mockAmenityService = new Mock<IAmenityService>();
            mockAmenityService.Setup(s => s.UpdateAsync(accommodationId, amenityIds)).Returns(Task.CompletedTask);

            var service = CreateService(
                accommodationRepo: mockAccommodationRepo.Object,
                mapper: mockMapper.Object,
                amenityService: mockAmenityService.Object);

            var resultId = await service.UpdateWithAmenitiesAsync(updateDto, amenityIds);

            Assert.Equal(accommodationId, resultId);
            mockMapper.Verify(m => m.Map<Accommodation>(updateDto), Times.Once);
            mockAccommodationRepo.Verify(r => r.UpdateAsync(mappedAccommodationEntity), Times.Once);
            mockAmenityService.Verify(s => s.UpdateAsync(accommodationId, amenityIds), Times.Once);
        }

        // 6. Tests that GetAllAsync returns a collection of all available accommodations as DTOs.
        [Fact]
        public async Task GetAllAsync_ReturnsAllAvailableAccommodationsAsDto()
        {
            var accommodations = new List<Accommodation>
            {
                new Accommodation { AccommodationId = 1, Title = "Acc1", UniversityId = 1, AccommodationTypeId = 1 },
                new Accommodation { AccommodationId = 2, Title = "Acc2", UniversityId = 2, AccommodationTypeId = 2 }
            };
            var dtos = new List<AccommodationDto>
            {
                new AccommodationDto { AccommodationId = 1, Title = "Acc1Dto" },
                new AccommodationDto { AccommodationId = 2, Title = "Acc2Dto" }
            };

            var mockAccommodationRepo = new Mock<IAccommodationRepository>();
            mockAccommodationRepo.Setup(r => r.GetAvailableAccommodationsAsync()).ReturnsAsync(accommodations);

            var mockAssembler = new Mock<IAccommodationAssemblerService>();
            mockAssembler.Setup(a => a.ToDtoAsync(accommodations[0])).ReturnsAsync(dtos[0]);
            mockAssembler.Setup(a => a.ToDtoAsync(accommodations[1])).ReturnsAsync(dtos[1]);

            var service = CreateService(
                accommodationRepo: mockAccommodationRepo.Object,
                assemblerService: mockAssembler.Object);

            var result = await service.GetAllAsync();

            Assert.NotNull(result);
            var resultList = result.ToList();
            Assert.Equal(2, resultList.Count);
            Assert.Contains(resultList, d => d.AccommodationId == 1 && d.Title == "Acc1Dto");
            Assert.Contains(resultList, d => d.AccommodationId == 2 && d.Title == "Acc2Dto");
            mockAccommodationRepo.Verify(r => r.GetAvailableAccommodationsAsync(), Times.Once);
            mockAssembler.Verify(a => a.ToDtoAsync(It.IsAny<Accommodation>()), Times.Exactly(2));
        }



        // 7. Tests - Edge Case - that GetAllAsync returns an empty list when there are no available accommodations.
        [Fact]
        public async Task GetAllAsync_ReturnsEmptyList_WhenNoAccommodationsAvailable()
        {
            var mockAccommodationRepo = new Mock<IAccommodationRepository>();
            mockAccommodationRepo.Setup(r => r.GetAvailableAccommodationsAsync()).ReturnsAsync(new List<Accommodation>());
            var mockAssembler = new Mock<IAccommodationAssemblerService>();

            var service = CreateService(
                accommodationRepo: mockAccommodationRepo.Object,
                assemblerService: mockAssembler.Object);

            var result = await service.GetAllAsync();

            Assert.NotNull(result);
            Assert.Empty(result);
            mockAccommodationRepo.Verify(r => r.GetAvailableAccommodationsAsync(), Times.Once);
            mockAssembler.Verify(a => a.ToDtoAsync(It.IsAny<Accommodation>()), Times.Never);
        }



        // 8. Tests that GetIndexAsync returns a collection of featured accommodations as DTOs.
        [Fact]
        public async Task GetIndexAsync_ReturnsFeaturedAccommodationsAsDto()
        {
            var accommodations = new List<Accommodation>
            {
                new Accommodation { AccommodationId = 1, Title = "Featured Acc1", UniversityId = 1, AccommodationTypeId = 1 },
            };
            var dtos = new List<AccommodationDto>
            {
                new AccommodationDto { AccommodationId = 1, Title = "Featured Acc1Dto" },
            };

            var mockAccommodationRepo = new Mock<IAccommodationRepository>();
            mockAccommodationRepo.Setup(r => r.GetFeaturedAccommodationsAsync()).ReturnsAsync(accommodations);

            var mockAssembler = new Mock<IAccommodationAssemblerService>();
            mockAssembler.Setup(a => a.ToDtoAsync(accommodations[0])).ReturnsAsync(dtos[0]);

            var service = CreateService(
                accommodationRepo: mockAccommodationRepo.Object,
                assemblerService: mockAssembler.Object);

            var result = await service.GetIndexAsync();

            Assert.NotNull(result);
            var resultList = result.ToList();
            Assert.Single(resultList);
            Assert.Equal(dtos[0].AccommodationId, resultList[0].AccommodationId);
            Assert.Equal(dtos[0].Title, resultList[0].Title);
            mockAccommodationRepo.Verify(r => r.GetFeaturedAccommodationsAsync(), Times.Once);
            mockAssembler.Verify(a => a.ToDtoAsync(It.IsAny<Accommodation>()), Times.Once);
        }



        // 9. Tests that GetByLandlordUserIdAsync returns a landlord's accommodations, correctly enriched with application counts and other details.
        [Fact]
        public async Task GetByLandlordUserIdAsync_ReturnsLandlordAccommodationsWithApplicationCounts()
        {
            var landlordUserId = "landlord123";
            var landlordId = 1;
            var landlord = new Landlord { LandlordId = landlordId, UserId = landlordUserId, FirstName = "John" };
            var accommodations = new List<Accommodation>
            {
                new Accommodation { AccommodationId = 101, LandlordId = landlordId, Title = "Acc A", UniversityId = 1, AccommodationTypeId = 1 },
                new Accommodation { AccommodationId = 102, LandlordId = landlordId, Title = "Acc B", UniversityId = 2, AccommodationTypeId = 2 }
            };
            var applicationCounts = new List<(int AccommodationId, int Count)>
            {
                (101, 5),
                (102, 2)
            };

            var mockLandlordRepo = new Mock<ILandlordRepository>();
            mockLandlordRepo.Setup(r => r.GetByUserIdAsync(landlordUserId)).ReturnsAsync(landlord);

            var mockAccommodationRepo = new Mock<IAccommodationRepository>();
            mockAccommodationRepo.Setup(r => r.GetAccommodationsByLandlordAsync(landlordId)).ReturnsAsync(accommodations);

            var mockApplicationService = new Mock<IApplicationService>();
            mockApplicationService.Setup(s => s.GetApplicationCountsByLandlordIdAsync(landlordId)).ReturnsAsync(applicationCounts);

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(m => m.Map<LandlordAccommodationDto>(accommodations[0])).Returns(new LandlordAccommodationDto { AccommodationId = 101, Title = "Acc A" });
            mockMapper.Setup(m => m.Map<LandlordAccommodationDto>(accommodations[1])).Returns(new LandlordAccommodationDto { AccommodationId = 102, Title = "Acc B" });

            var mockAmenityService = new Mock<IAmenityService>();
            mockAmenityService.Setup(s => s.GetNamesByAccommodationIdAsync(101)).ReturnsAsync(new List<string> { "Wifi", "Pool" });
            mockAmenityService.Setup(s => s.GetNamesByAccommodationIdAsync(102)).ReturnsAsync(new List<string> { "Gym" });

            var mockImageService = new Mock<IAccommodationImageService>();
            mockImageService.Setup(s => s.GetUrlsByAccommodationIdAsync(101)).ReturnsAsync(new List<string> { "url1", "url2" });
            mockImageService.Setup(s => s.GetUrlsByAccommodationIdAsync(102)).ReturnsAsync(new List<string> { "url3" });

            var mockUniversityService = new Mock<IUniversityService>();
            mockUniversityService.Setup(s => s.GetNameByIdAsync(1)).ReturnsAsync("University Alpha");
            mockUniversityService.Setup(s => s.GetNameByIdAsync(2)).ReturnsAsync("University Beta");

            var mockTypeService = new Mock<IAccommodationTypeService>();
            mockTypeService.Setup(s => s.GetNameByIdAsync(1)).ReturnsAsync("Apartment");
            mockTypeService.Setup(s => s.GetNameByIdAsync(2)).ReturnsAsync("House");

            var service = CreateService(
                landlordRepo: mockLandlordRepo.Object,
                accommodationRepo: mockAccommodationRepo.Object,
                applicationService: mockApplicationService.Object,
                mapper: mockMapper.Object,
                amenityService: mockAmenityService.Object,
                imageService: mockImageService.Object,
                universityService: mockUniversityService.Object,
                typeService: mockTypeService.Object);

            var results = await service.GetByLandlordUserIdAsync(landlordUserId);

            Assert.NotNull(results);
            var resultList = results.ToList();
            Assert.Equal(2, resultList.Count);

            var acc1 = resultList.FirstOrDefault(a => a.AccommodationId == 101);
            Assert.NotNull(acc1);
            Assert.Equal("Acc A", acc1.Title);
            Assert.Equal(5, acc1.ApplicationCount);
            Assert.Contains("Wifi", acc1.AmenityNames);
            Assert.Contains("Pool", acc1.AmenityNames);
            Assert.Contains("url1", acc1.ImageUrls);
            Assert.Equal("University Alpha", acc1.UniversityName);
            Assert.Equal("Apartment", acc1.AccommodationType);

            var acc2 = resultList.FirstOrDefault(a => a.AccommodationId == 102);
            Assert.NotNull(acc2);
            Assert.Equal("Acc B", acc2.Title);
            Assert.Equal(2, acc2.ApplicationCount);
            Assert.Contains("Gym", acc2.AmenityNames);
            Assert.Contains("url3", acc2.ImageUrls);
            Assert.Equal("University Beta", acc2.UniversityName);
            Assert.Equal("House", acc2.AccommodationType);

            mockLandlordRepo.Verify(r => r.GetByUserIdAsync(landlordUserId), Times.Once);
            mockAccommodationRepo.Verify(r => r.GetAccommodationsByLandlordAsync(landlordId), Times.Once);
            mockApplicationService.Verify(s => s.GetApplicationCountsByLandlordIdAsync(landlordId), Times.Once);
            mockMapper.Verify(m => m.Map<LandlordAccommodationDto>(It.IsAny<Accommodation>()), Times.Exactly(2));
            mockAmenityService.Verify(s => s.GetNamesByAccommodationIdAsync(It.IsAny<int>()), Times.Exactly(2));
            mockImageService.Verify(s => s.GetUrlsByAccommodationIdAsync(It.IsAny<int>()), Times.Exactly(2));
            mockUniversityService.Verify(s => s.GetNameByIdAsync(It.IsAny<int>()), Times.Exactly(2));
            mockTypeService.Verify(s => s.GetNameByIdAsync(It.IsAny<int>()), Times.Exactly(2));
        }



        // 10. Tests - Edge Case - that GetByLandlordUserIdAsync throws a NotFoundException if the landlord's user ID does not exist.
        [Fact]
        public async Task GetByLandlordUserIdAsync_ThrowsNotFoundException_WhenLandlordDoesNotExist()
        {
            var landlordUserId = "nonexistent_landlord";
            var mockLandlordRepo = new Mock<ILandlordRepository>();
            mockLandlordRepo.Setup(r => r.GetByUserIdAsync(landlordUserId)).ReturnsAsync((Landlord)null);
            var mockAccommodationRepo = new Mock<IAccommodationRepository>();
            var mockApplicationService = new Mock<IApplicationService>();

            var service = CreateService(
                landlordRepo: mockLandlordRepo.Object,
                accommodationRepo: mockAccommodationRepo.Object,
                applicationService: mockApplicationService.Object);

            var exception = await Assert.ThrowsAsync<NotFoundException>(() => service.GetByLandlordUserIdAsync(landlordUserId));
            Assert.Equal($"No landlord found for user ID {landlordUserId}", exception.Message);
            mockLandlordRepo.Verify(r => r.GetByUserIdAsync(landlordUserId), Times.Once);
            mockAccommodationRepo.Verify(r => r.GetAccommodationsByLandlordAsync(It.IsAny<int>()), Times.Never);
            mockApplicationService.Verify(s => s.GetApplicationCountsByLandlordIdAsync(It.IsAny<int>()), Times.Never);
        }



        // 11. Tests that GetApplicationByStudentUserIdAsync returns all accommodations a student has applied for, with correct status.
        [Fact]
        public async Task GetApplicationByStudentUserIdAsync_ReturnsAppliedAccommodations()
        {
            var studentUserId = "student123";
            var studentId = 1;
            var student = new Student { StudentId = studentId, UserId = studentUserId };
            var appTuples = new List<(int ApplicationId, int AccommodationId)>
            {
                (1001, 10),
                (1002, 11)
            };
            var acc1 = new Accommodation { AccommodationId = 10, Title = "Student Acc 1", UniversityId = 1, AccommodationTypeId = 1 };
            var acc2 = new Accommodation { AccommodationId = 11, Title = "Student Acc 2", UniversityId = 2, AccommodationTypeId = 2 };
            var accDto1 = new AccommodationDto { AccommodationId = 10, Title = "Student Acc 1 DTO", UniversityName = "Uni X", AccommodationType = "Apt" };
            var accDto2 = new AccommodationDto { AccommodationId = 11, Title = "Student Acc 2 DTO", UniversityName = "Uni Y", AccommodationType = "House" };

            var mockStudentRepo = new Mock<IStudentRepository>();
            mockStudentRepo.Setup(r => r.GetByUserIdAsync(studentUserId)).ReturnsAsync(student);

            var mockApplicationService = new Mock<IApplicationService>();
            mockApplicationService.Setup(s => s.GetApplicationsWithAccommodationIdsByStudentAsync(studentId)).ReturnsAsync(appTuples);
            mockApplicationService.Setup(s => s.GetStatusNameByStudentAndAccommodationIdAsync(studentId, 10)).ReturnsAsync("Accepted");
            mockApplicationService.Setup(s => s.GetStatusNameByStudentAndAccommodationIdAsync(studentId, 11)).ReturnsAsync("Pending");

            var mockAccommodationRepo = new Mock<IAccommodationRepository>();
            mockAccommodationRepo.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(acc1);
            mockAccommodationRepo.Setup(r => r.GetByIdAsync(11)).ReturnsAsync(acc2);

            var mockAssembler = new Mock<IAccommodationAssemblerService>();
            mockAssembler.Setup(a => a.ToDtoAsync(acc1)).ReturnsAsync(accDto1);
            mockAssembler.Setup(a => a.ToDtoAsync(acc2)).ReturnsAsync(accDto2);

            var service = CreateService(
                studentRepo: mockStudentRepo.Object,
                applicationService: mockApplicationService.Object,
                accommodationRepo: mockAccommodationRepo.Object,
                assemblerService: mockAssembler.Object);

            var results = await service.GetApplicationByStudentUserIdAsync(studentUserId);

            Assert.NotNull(results);
            var resultList = results.ToList();
            Assert.Equal(2, resultList.Count);

            var appliedAcc1 = resultList.FirstOrDefault(a => a.ApplicationId == 1001);
            Assert.NotNull(appliedAcc1);
            Assert.Equal(10, appliedAcc1.AccommodationId);
            Assert.Equal("Student Acc 1 DTO", appliedAcc1.Title);
            Assert.Equal("Accepted", appliedAcc1.ApplicationStatus);

            var appliedAcc2 = resultList.FirstOrDefault(a => a.ApplicationId == 1002);
            Assert.NotNull(appliedAcc2);
            Assert.Equal(11, appliedAcc2.AccommodationId);
            Assert.Equal("Student Acc 2 DTO", appliedAcc2.Title);
            Assert.Equal("Pending", appliedAcc2.ApplicationStatus);

            mockStudentRepo.Verify(r => r.GetByUserIdAsync(studentUserId), Times.Once);
            mockApplicationService.Verify(s => s.GetApplicationsWithAccommodationIdsByStudentAsync(studentId), Times.Once);
            mockAccommodationRepo.Verify(r => r.GetByIdAsync(It.IsAny<int>()), Times.Exactly(2));
            mockAssembler.Verify(a => a.ToDtoAsync(It.IsAny<Accommodation>()), Times.Exactly(2));
            mockApplicationService.Verify(s => s.GetStatusNameByStudentAndAccommodationIdAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Exactly(2));
        }



        // 12. Tests - Edge Case - that GetApplicationByStudentUserIdAsync returns an empty list for a student who has not made any applications.
        [Fact]
        public async Task GetApplicationByStudentUserIdAsync_ReturnsEmptyList_WhenStudentHasNoApplications()
        {
            var studentUserId = "student123";
            var studentId = 1;
            var student = new Student { StudentId = studentId, UserId = studentUserId };

            var mockStudentRepo = new Mock<IStudentRepository>();
            mockStudentRepo.Setup(r => r.GetByUserIdAsync(studentUserId)).ReturnsAsync(student);

            var mockApplicationService = new Mock<IApplicationService>();
            mockApplicationService.Setup(s => s.GetApplicationsWithAccommodationIdsByStudentAsync(studentId)).ReturnsAsync(new List<(int, int)>());

            var mockAccommodationRepo = new Mock<IAccommodationRepository>();
            var mockAssembler = new Mock<IAccommodationAssemblerService>();

            var service = CreateService(
                studentRepo: mockStudentRepo.Object,
                applicationService: mockApplicationService.Object,
                accommodationRepo: mockAccommodationRepo.Object,
                assemblerService: mockAssembler.Object);

            var results = await service.GetApplicationByStudentUserIdAsync(studentUserId);

            Assert.NotNull(results);
            Assert.Empty(results);
            mockStudentRepo.Verify(r => r.GetByUserIdAsync(studentUserId), Times.Once);
            mockApplicationService.Verify(s => s.GetApplicationsWithAccommodationIdsByStudentAsync(studentId), Times.Once);
            mockAccommodationRepo.Verify(r => r.GetByIdAsync(It.IsAny<int>()), Times.Never);
            mockAssembler.Verify(a => a.ToDtoAsync(It.IsAny<Accommodation>()), Times.Never);
        }

        // 13. Tests that GetBookingsByStudentUserIdAsync returns all accommodations a student has booked, with correct status.
        [Fact]
        public async Task GetBookingsByStudentUserIdAsync_ReturnsBookedAccommodations_WhenBookingsExist()
        {
            var studentUserId = "student456";
            var studentId = 2;
            var student = new Student { StudentId = studentId, UserId = studentUserId };
            var bookings = new List<BookingDto>
            {
                new BookingDto { BookingId = 2001, AccommodationId = 20, StatusId = 1 },
                new BookingDto { BookingId = 2002, AccommodationId = 21, StatusId = 2 }
            };
            var acc1 = new Accommodation { AccommodationId = 20, Title = "Booked Acc 1" };
            var acc2 = new Accommodation { AccommodationId = 21, Title = "Booked Acc 2" };
            var accDto1 = new AccommodationDto { AccommodationId = 20, Title = "Booked Acc 1 DTO" };
            var accDto2 = new AccommodationDto { AccommodationId = 21, Title = "Booked Acc 2 DTO" };

            var mockStudentRepo = new Mock<IStudentRepository>();
            mockStudentRepo.Setup(r => r.GetByUserIdAsync(studentUserId)).ReturnsAsync(student);

            var mockBookingService = new Mock<IBookingService>();
            mockBookingService.Setup(s => s.GetByStudentAsync(studentId)).ReturnsAsync(bookings);
            mockBookingService.Setup(s => s.GetStatusNameAsync(1)).ReturnsAsync("Confirmed");
            mockBookingService.Setup(s => s.GetStatusNameAsync(2)).ReturnsAsync("Completed");

            var mockAccommodationRepo = new Mock<IAccommodationRepository>();
            mockAccommodationRepo.Setup(r => r.GetByIdAsync(20)).ReturnsAsync(acc1);
            mockAccommodationRepo.Setup(r => r.GetByIdAsync(21)).ReturnsAsync(acc2);

            var mockAssembler = new Mock<IAccommodationAssemblerService>();
            mockAssembler.Setup(a => a.ToDtoAsync(acc1)).ReturnsAsync(accDto1);
            mockAssembler.Setup(a => a.ToDtoAsync(acc2)).ReturnsAsync(accDto2);

            var service = CreateService(
                studentRepo: mockStudentRepo.Object,
                bookingService: mockBookingService.Object,
                accommodationRepo: mockAccommodationRepo.Object,
                assemblerService: mockAssembler.Object);

            var results = await service.GetBookingsByStudentUserIdAsync(studentUserId);

            Assert.NotNull(results);
            var resultList = results.ToList();
            Assert.Equal(2, resultList.Count);

            var booking1 = resultList.FirstOrDefault(b => b.BookingId == 2001);
            Assert.NotNull(booking1);
            Assert.Equal(20, booking1.AccommodationId);
            Assert.Equal("Booked Acc 1 DTO", booking1.Title);
            Assert.Equal("Confirmed", booking1.BookingStatus);

            var booking2 = resultList.FirstOrDefault(b => b.BookingId == 2002);
            Assert.NotNull(booking2);
            Assert.Equal(21, booking2.AccommodationId);
            Assert.Equal("Booked Acc 2 DTO", booking2.Title);
            Assert.Equal("Completed", booking2.BookingStatus);

            mockStudentRepo.Verify(r => r.GetByUserIdAsync(studentUserId), Times.Once);
            mockBookingService.Verify(s => s.GetByStudentAsync(studentId), Times.Once);
            mockAccommodationRepo.Verify(r => r.GetByIdAsync(It.IsAny<int>()), Times.Exactly(2));
            mockAssembler.Verify(a => a.ToDtoAsync(It.IsAny<Accommodation>()), Times.Exactly(2));
            mockBookingService.Verify(s => s.GetStatusNameAsync(It.IsAny<int>()), Times.Exactly(2));
        }

        // 14. Tests - Edge Case - that GetBookingsByStudentUserIdAsync returns an empty list if the student does not exist.
        [Fact]
        public async Task GetBookingsByStudentUserIdAsync_ReturnsEmptyList_WhenStudentDoesNotExist()
        {
            var studentUserId = "nonexistent_student";
            var mockStudentRepo = new Mock<IStudentRepository>();
            mockStudentRepo.Setup(r => r.GetByUserIdAsync(studentUserId)).ReturnsAsync((Student)null);
            var mockBookingService = new Mock<IBookingService>();

            var service = CreateService(
                studentRepo: mockStudentRepo.Object,
                bookingService: mockBookingService.Object);

            var results = await service.GetBookingsByStudentUserIdAsync(studentUserId);

            Assert.NotNull(results);
            Assert.Empty(results);
            mockStudentRepo.Verify(r => r.GetByUserIdAsync(studentUserId), Times.Once);
            mockBookingService.Verify(s => s.GetByStudentAsync(It.IsAny<int>()), Times.Never);
        }
    }
}