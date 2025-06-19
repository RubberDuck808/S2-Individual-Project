using BLL.Interfaces;
using BLL.DTOs.Student;
using Microsoft.AspNetCore.Identity;
using DAL.Interfaces;
using Domain.Models;
using BLL.DTOs.Landlord;
using Microsoft.Extensions.Logging;
using BLL.Services;

public class AccountService : IAccountService
{
    private readonly IUserRepository _userRepository;
    private readonly IUniversityService _universityService;
    private readonly IStudentService _studentService;
    private readonly ILandlordService _landlordService;
    private readonly IPasswordHasher<object> _hasher;
    private readonly ILogger<AccountService> _logger;

    public AccountService(
        IUserRepository userRepository,
        IUniversityService universityService,
        IStudentService studentService,
        ILandlordService landlordService,
        IPasswordHasher<object> hasher,
        ILogger<AccountService> logger)
    {
        _userRepository = userRepository;
        _universityService = universityService;
        _studentService = studentService;
        _landlordService = landlordService;
        _hasher = hasher;
        _logger = logger;
    }




    public async Task RegisterStudentAsync(StudentRegistrationDto dto)
    {
        _logger.LogInformation("Registering student with email: {Email}", dto.Email);

        var domain = dto.Email.Split('@').Last();
        var universityId = await _universityService.GetUniversityIdByDomainAsync(domain); // this is too check if the domain is in the db

        dto.UniversityId = universityId;

        var passwordHash = _hasher.HashPassword(null, dto.Password);

        var userId = await _userRepository.CreateUserAsync(dto.Email, passwordHash, dto.PhoneNumber, dto.FirstName, dto.LastName);
        _logger.LogInformation("User created with ID: {UserId}", userId);

        await _studentService.CreateStudentAsync(userId, dto);
        _logger.LogInformation("Student record created for user ID: {UserId}", userId);

        await _userRepository.AssignRoleAsync(userId, "Student");
        _logger.LogInformation("Student role assigned to user ID: {UserId}", userId);
    }


    public async Task RegisterLandlordAsync(LandlordRegistrationDto dto)
    {
        _logger.LogInformation("Registering landlord with email: {Email}", dto.Email);

        var passwordHash = _hasher.HashPassword(null, dto.Password);

        var userId = await _userRepository.CreateUserAsync(
            dto.Email, passwordHash, dto.PhoneNumber, dto.FirstName, dto.LastName);
        _logger.LogInformation("User created with ID: {UserId}", userId);

        await _landlordService.CreateLandlordAsync(userId, dto);
        _logger.LogInformation("Landlord record created for user ID: {UserId}", userId);

        await _userRepository.AssignRoleAsync(userId, "Landlord");
        _logger.LogInformation("Landlord role assigned to user ID: {UserId}", userId);
    }



    public async Task<(bool Success, string? UserId, string? Role, string? Error)> LoginAsync(string email, string password)
    {
        _logger.LogInformation("Attempting login for email: {Email}", email);

        try
        {
            var (userId, storedHash) = await _userRepository.GetUserAuthDataAsync(email);

            var verificationResult = _hasher.VerifyHashedPassword(null, storedHash, password);
            if (verificationResult == PasswordVerificationResult.Failed)
            {
                _logger.LogWarning("Password verification failed for email: {Email}", email);
                return (false, null, null, "Invalid credentials.");
            }

            var roles = await _userRepository.GetRolesAsync(userId);
            var primaryRole = roles.FirstOrDefault();

            if (primaryRole == null)
            {
                _logger.LogWarning("User with ID {UserId} has no roles assigned", userId);
                return (false, null, null, "No role assigned to this user.");
            }

            _logger.LogInformation("Login successful for user ID: {UserId}, role: {Role}", userId, primaryRole);
            return (true, userId, primaryRole, null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Login failed for email: {Email}", email);
            return (false, null, null, ex.Message);
        }
    }

    public async Task<string> GetFullNameByUserIdAsync(string userId)
    {
        _logger.LogInformation("Retrieving full name for user ID: {UserId}", userId);

        var landlord = await _landlordService.GetByUserIdAsync(userId);
        if (landlord != null)
        {
            var middle = string.IsNullOrEmpty(landlord.MiddleName) ? "" : $" {landlord.MiddleName}";
            var name = $"{landlord.FirstName}{middle} {landlord.LastName}";
            _logger.LogInformation("Found landlord name: {FullName}", name);
            return name;
        }

        var student = await _studentService.GetByUserIdAsync(userId);
        if (student != null)
        {
            var middle = string.IsNullOrEmpty(student.MiddleName) ? "" : $" {student.MiddleName}";
            var name = $"{student.FirstName}{middle} {student.LastName}";
            _logger.LogInformation("Found student name: {FullName}", name);
            return name;
        }

        _logger.LogWarning("No matching user found for ID: {UserId}", userId);
        return "User";
    }
}
