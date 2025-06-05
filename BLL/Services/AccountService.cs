using BLL.Interfaces;
using BLL.DTOs.Student;
using Microsoft.AspNetCore.Identity;
using DAL.Interfaces;
using DAL.Models;
using BLL.DTOs.Landlord;

public class AccountService : IAccountService
{
    private readonly PasswordHasher<object> _hasher = new();
    private readonly IUserRepository _userRepository;
    private readonly IUniversityRepository _universityRepository;
    private readonly IStudentRepository _studentRepository;

    public AccountService(
        IUserRepository userRepository,
        IUniversityRepository universityRepository,
        IStudentRepository studentRepository)
    {
        _userRepository = userRepository;
        _universityRepository = universityRepository;
        _studentRepository = studentRepository;
    }

    public async Task RegisterStudentAsync(StudentRegistrationDto dto)
    {
        Console.WriteLine("Entered AccountService.RegisterStudentAsync");
        // 1. Validate email domain
        var domain = dto.Email.Split('@').Last();
        var universityId = await _universityRepository.GetUniversityIdByEmailDomainAsync(domain);
        if (universityId == null)
            throw new Exception("Invalid student email domain.");
        dto.UniversityId = universityId.Value;

        // 2. Hash password
        var passwordHash = _hasher.HashPassword(null, dto.Password);

        // 3. Create user
        var userId = await _userRepository.CreateUserAsync(dto.Email, passwordHash, dto.PhoneNumber, dto.FirstName, dto.LastName);

        // 4. Map and create student
        var student = new Student
        {
            UserId = userId,
            UniversityId = dto.UniversityId,
            Email = dto.Email,
            FirstName = dto.FirstName,
            MiddleName = dto.MiddleName ?? "",
            LastName = dto.LastName,
            DateOfBirth = dto.DateOfBirth,
            PhoneNumber = dto.PhoneNumber,
            EmergencyContact = dto.EmergencyContact ?? "",
            EmergencyPhone = dto.EmergencyPhone ?? "",
            ProfileImageUrl = dto.ProfileImageUrl,
            IsVerified = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _studentRepository.AddAsync(student);

        // 5. Assign role
        await _userRepository.AssignRoleAsync(userId, "Student");
    }

    public Task RegisterLandlordAsync(LandlordRegistrationDto dto)
    {
        throw new NotImplementedException();
    }

}
