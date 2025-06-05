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
    private readonly ILandlordRepository _landlordRepository;


    public AccountService(
        IUserRepository userRepository,
        IUniversityRepository universityRepository,
        IStudentRepository studentRepository,
        ILandlordRepository landlordRepository)
    {
        _userRepository = userRepository;
        _universityRepository = universityRepository;
        _studentRepository = studentRepository;
        _landlordRepository = landlordRepository;
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

    public async Task RegisterLandlordAsync(LandlordRegistrationDto dto)
    {
        Console.WriteLine("Entered AccountService.RegisterLandlordAsync");

        // 1. Hash password
        var passwordHash = _hasher.HashPassword(null, dto.Password);

        // 2. Create user
        var userId = await _userRepository.CreateUserAsync(dto.Email, passwordHash, dto.PhoneNumber, dto.FirstName, dto.LastName);

        // 3. Map and create landlord
        var landlord = new Landlord
        {
            UserId = userId,
            FirstName = dto.FirstName,
            MiddleName = dto.MiddleName ?? string.Empty,
            LastName = dto.LastName,
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber,
            CompanyName = dto.CompanyName,
            TaxIdentificationNumber = dto.TaxIdentificationNumber,
            IsVerified = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _landlordRepository.AddAsync(landlord);

        // 5. Assign role
        await _userRepository.AssignRoleAsync(userId, "Landlord");
    }

    public async Task<(bool Success, string? UserId, string? Role, string? Error)> LoginAsync(string email, string password)
    {
        try
        {
            var (userId, storedHash) = await _userRepository.GetUserAuthDataAsync(email);

            var verificationResult = _hasher.VerifyHashedPassword(null, storedHash, password);
            if (verificationResult == PasswordVerificationResult.Failed)
                return (false, null, null, "Invalid credentials.");

            var roles = await _userRepository.GetRolesAsync(userId);
            var primaryRole = roles.FirstOrDefault();

            if (primaryRole == null)
                return (false, null, null, "No role assigned to this user.");

            return (true, userId, primaryRole, null);
        }
        catch (Exception ex)
        {
            return (false, null, null, ex.Message);
        }


    }
    public async Task<string> GetFullNameByUserIdAsync(string userId)
    {
        // Try landlord first
        var landlord = await _landlordRepository.GetByUserIdAsync(userId);
        if (landlord != null)
        {
            // Concatenate first, middle (if any), last name
            var middle = string.IsNullOrEmpty(landlord.MiddleName) ? "" : $" {landlord.MiddleName}";
            return $"{landlord.FirstName}{middle} {landlord.LastName}";
        }

        // Try student next
        var student = await _studentRepository.GetByUserIdAsync(userId);
        if (student != null)
        {
            var middle = string.IsNullOrEmpty(student.MiddleName) ? "" : $" {student.MiddleName}";
            return $"{student.FirstName}{middle} {student.LastName}";
        }

        // Fallback or unknown user
        return "User";
    }


}
