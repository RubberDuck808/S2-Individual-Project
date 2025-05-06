using UniNest.BLL.DTOs.Student;

namespace UniNest.BLL.Interfaces
{
    public interface IStudentService
    {
        Task<StudentDto> GetByIdAsync(int id);
        Task<int> CreateAsync(StudentCreateDto dto);
        Task UpdateAsync(int studentId, StudentUpdateDto dto);
    }
}
