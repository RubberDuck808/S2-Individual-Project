using AutoMapper;
using BLL.DTOs.Student;
using BLL.Exceptions;
using BLL.Services;
using DAL.Interfaces;
using Domain.Models;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Tests
{
    public class StudentServiceTests
    {
        private readonly Mock<IStudentRepository> _mockRepo;
        private readonly Mock<ILogger<StudentService>> _mockLogger;
        private readonly IMapper _mapper;
        private readonly StudentService _service;

        public StudentServiceTests()
        {
            _mockRepo = new Mock<IStudentRepository>();
            _mockLogger = new Mock<ILogger<StudentService>>();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Student, StudentDto>();
                cfg.CreateMap<StudentRegistrationDto, Student>();
                cfg.CreateMap<StudentUpdateDto, Student>();
            });
            _mapper = config.CreateMapper();

            _service = new StudentService(_mockRepo.Object, _mapper, _mockLogger.Object);
        }



        // 1. Tests GetByIdAsync returns mapped DTO
        [Fact]
        public async Task GetByIdAsync_ReturnsStudentDto_WhenFound()
        {
            var student = new Student { StudentId = 1, FirstName = "Jane" };
            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(student);

            var result = await _service.GetByIdAsync(1);

            Assert.Equal(1, result.StudentId);
            Assert.Equal("Jane", result.FirstName);
        }



        // 2. Tests GetByIdAsync throws if student not found
        [Fact]
        public async Task GetByIdAsync_ThrowsNotFound_WhenMissing()
        {
            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Student?)null);

            await Assert.ThrowsAsync<NotFoundException>(() => _service.GetByIdAsync(1));
        }



        // 3. Tests CreateAsync maps and returns ID
        [Fact]
        public async Task CreateAsync_ReturnsNewId_WhenSuccessful()
        {
            var dto = new StudentRegistrationDto { Email = "test@example.com", FirstName = "John", LastName = "Doe" };
            _mockRepo.Setup(r => r.AddAsync(It.IsAny<Student>()))
                     .Callback<Student>(s => s.StudentId = 42)
                     .Returns(() => Task.FromResult(0));

            var id = await _service.CreateAsync(dto);

            Assert.Equal(42, id);
        }



        // 4. Tests UpdateAsync modifies existing student
        [Fact]
        public async Task UpdateAsync_UpdatesStudent_WhenFound()
        {
            var existing = new Student { StudentId = 1, FirstName = "Old" };
            var dto = new StudentUpdateDto { FirstName = "New" };

            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);
            _mockRepo.Setup(r => r.UpdateAsync(existing)).Returns(Task.CompletedTask);

            await _service.UpdateAsync(1, dto);

            Assert.Equal("New", existing.FirstName);
            _mockRepo.Verify(r => r.UpdateAsync(existing), Times.Once);
        }



        // 5. Tests UpdateAsync throws if student not found
        [Fact]
        public async Task UpdateAsync_ThrowsNotFound_WhenMissing()
        {
            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Student?)null);

            var dto = new StudentUpdateDto { FirstName = "Fail" };
            await Assert.ThrowsAsync<NotFoundException>(() => _service.UpdateAsync(1, dto));
        }

        // 6. Tests GetByUserIdAsync returns DTO
        [Fact]
        public async Task GetByUserIdAsync_ReturnsStudentDto_WhenFound()
        {
            var student = new Student { UserId = "abc123", FirstName = "Anna" };
            _mockRepo.Setup(r => r.GetByUserIdAsync("abc123")).ReturnsAsync(student);

            var result = await _service.GetByUserIdAsync("abc123");

            Assert.NotNull(result);
            Assert.Equal("Anna", result.FirstName);
        }



        // 7. Tests GetByUserIdAsync returns null when not found
        [Fact]
        public async Task GetByUserIdAsync_ReturnsNull_WhenMissing()
        {
            _mockRepo.Setup(r => r.GetByUserIdAsync("unknown")).ReturnsAsync((Student?)null);

            var result = await _service.GetByUserIdAsync("unknown");

            Assert.Null(result);
        }



        // 8. Tests CreateStudentAsync creates correct student
        [Fact]
        public async Task CreateStudentAsync_MapsCorrectly_AndCallsAdd()
        {
            var dto = new StudentRegistrationDto
            {
                Email = "s@x.com",
                FirstName = "F",
                LastName = "L",
                UniversityId = 1,
                DateOfBirth = new DateTime(2000, 1, 1),
                PhoneNumber = "123"
            };

            await _service.CreateStudentAsync("user-123", dto);

            _mockRepo.Verify(r => r.AddAsync(It.Is<Student>(s =>
                s.UserId == "user-123" &&
                s.Email == dto.Email &&
                s.FirstName == dto.FirstName &&
                s.UniversityId == 1 &&
                s.IsVerified == false
            )), Times.Once);
        }
    }
}
