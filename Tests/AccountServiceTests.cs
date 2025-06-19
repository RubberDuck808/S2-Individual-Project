using BLL.DTOs.Landlord;
using BLL.DTOs.Student;
using BLL.Interfaces;
using DAL.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;

namespace Tests
{
    public class AccountServiceTests
    {
        [Fact]
        public async Task RegisterStudentAsync_CreatesUser_CreatesStudent_AssignsRole()
        {
            // Arrange
            var dto = new StudentRegistrationDto
            {
                Email = "john@student.uni.nl",
                Password = "Test123!",
                FirstName = "John",
                LastName = "Doe",
                PhoneNumber = "0612345678",
                DateOfBirth = new DateTime(2000, 1, 1)
            };

            var mockUserRepo = new Mock<IUserRepository>();
            var mockStudentService = new Mock<IStudentService>();
            var mockUniversityService = new Mock<IUniversityService>();
            var mockHasher = new Mock<IPasswordHasher<object>>();
            var mockLogger = new Mock<ILogger<AccountService>>();

            mockHasher.Setup(h => h.HashPassword(null, dto.Password)).Returns("hashed-pw");
            mockUniversityService.Setup(u => u.GetUniversityIdByDomainAsync("student.uni.nl")).ReturnsAsync(1);
            mockUserRepo.Setup(r => r.CreateUserAsync(dto.Email, "hashed-pw", dto.PhoneNumber, dto.FirstName, dto.LastName))
                        .ReturnsAsync("user-id-123");

            var accountService = new AccountService(
                mockUserRepo.Object,                      
                mockUniversityService.Object,             
                mockStudentService.Object,                
                Mock.Of<ILandlordService>(),             
                mockHasher.Object,                        
                mockLogger.Object                        
            );


            // Act
            await accountService.RegisterStudentAsync(dto);

            // Assert
            mockUniversityService.Verify(u => u.GetUniversityIdByDomainAsync("student.uni.nl"), Times.Once);
            mockUserRepo.Verify(r => r.CreateUserAsync(dto.Email, "hashed-pw", dto.PhoneNumber, dto.FirstName, dto.LastName), Times.Once);
            mockStudentService.Verify(s => s.CreateStudentAsync("user-id-123", dto), Times.Once);
            mockUserRepo.Verify(r => r.AssignRoleAsync("user-id-123", "Student"), Times.Once);
        }


        [Fact]
        public async Task RegisterStudentAsync_ThrowsException_WhenDomainNotFound()
        {
            var dto = new StudentRegistrationDto
            {
                Email = "alice@unknown.com",
                Password = "pass",
                FirstName = "Alice",
                LastName = "Doe",
                PhoneNumber = "0600000000",
                DateOfBirth = new DateTime(2000, 1, 1)
            };

            var mockUniversityService = new Mock<IUniversityService>();
            mockUniversityService.Setup(s => s.GetUniversityIdByDomainAsync("unknown.com"))
                                 .ThrowsAsync(new Exception("No university found"));

            var accountService = new AccountService(
                Mock.Of<IUserRepository>(),
                mockUniversityService.Object,
                Mock.Of<IStudentService>(),
                Mock.Of<ILandlordService>(),
                Mock.Of<IPasswordHasher<object>>(),
                Mock.Of<ILogger<AccountService>>()
            );

            var ex = await Assert.ThrowsAsync<Exception>(() => accountService.RegisterStudentAsync(dto));
            Assert.Equal("No university found", ex.Message);
        }


        [Fact]
        public async Task RegisterLandlordAsync_CreatesUser_CreatesLandlord_AssignsRole()
        {
            // Arrange
            var dto = new LandlordRegistrationDto
            {
                Email = "landlord@uninest.nl",
                Password = "SecurePass!",
                FirstName = "Anna",
                LastName = "Smith",
                PhoneNumber = "0612345678",
                CompanyName = "RentCo",
                TaxIdentificationNumber = "NL123456789"
            };

            var mockUserRepo = new Mock<IUserRepository>();
            var mockLandlordService = new Mock<ILandlordService>();
            var mockHasher = new Mock<IPasswordHasher<object>>();
            var mockLogger = new Mock<ILogger<AccountService>>();

            // Setup mock behaviors
            mockHasher.Setup(h => h.HashPassword(null, dto.Password)).Returns("hashed-password");
            mockUserRepo.Setup(r => r.CreateUserAsync(dto.Email, "hashed-password", dto.PhoneNumber, dto.FirstName, dto.LastName))
                        .ReturnsAsync("user-id-456");

            var accountService = new AccountService(
                mockUserRepo.Object,
                Mock.Of<IUniversityService>(), // Not used in landlord registration
                Mock.Of<IStudentService>(),    // Not used here either
                mockLandlordService.Object,
                mockHasher.Object,
                mockLogger.Object
            );

            // Act
            await accountService.RegisterLandlordAsync(dto);

            // Assert
            mockUserRepo.Verify(r => r.CreateUserAsync(dto.Email, "hashed-password", dto.PhoneNumber, dto.FirstName, dto.LastName), Times.Once);
            mockLandlordService.Verify(l => l.CreateLandlordAsync("user-id-456", dto), Times.Once);
            mockUserRepo.Verify(r => r.AssignRoleAsync("user-id-456", "Landlord"), Times.Once);
        }

        [Fact]
        public async Task RegisterStudentAsync_Throws_WhenUserCreationFails()
        {
            var dto = new StudentRegistrationDto
            {
                Email = "john@student.uni.nl",
                Password = "pass",
                FirstName = "John",
                LastName = "Doe",
                PhoneNumber = "0612345678",
                DateOfBirth = DateTime.Now
            };

            var mockUserRepo = new Mock<IUserRepository>();
            var mockUniversityService = new Mock<IUniversityService>();
            var mockHasher = new Mock<IPasswordHasher<object>>();

            mockUniversityService.Setup(u => u.GetUniversityIdByDomainAsync("student.uni.nl")).ReturnsAsync(1);
            mockHasher.Setup(h => h.HashPassword(null, dto.Password)).Returns("hashed-pw");
            mockUserRepo.Setup(u => u.CreateUserAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                        .ThrowsAsync(new Exception("DB unavailable"));

            var accountService = new AccountService(
                mockUserRepo.Object,
                mockUniversityService.Object,
                Mock.Of<IStudentService>(),
                Mock.Of<ILandlordService>(),
                mockHasher.Object,
                Mock.Of<ILogger<AccountService>>()
            );

            var ex = await Assert.ThrowsAsync<Exception>(() => accountService.RegisterStudentAsync(dto));
            Assert.Equal("DB unavailable", ex.Message);
        }


    }
}