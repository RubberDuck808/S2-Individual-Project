using BLL.DTOs.Student;

namespace BLL.Interfaces
{
    public interface IStudentService
    {
        Task<StudentDto> GetByIdAsync(int id);
        Task<int> CreateAsync(StudentRegistrationDto dto);
        Task UpdateAsync(int studentId, StudentUpdateDto dto);
        Task<StudentDto?> GetByUserIdAsync(string userId);
    }
}
