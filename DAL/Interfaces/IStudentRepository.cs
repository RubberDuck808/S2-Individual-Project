using Domain.Models;

namespace DAL.Interfaces
{
    public interface IStudentRepository
    {
        Task<Student?> GetByIdAsync(int id);
        Task<Student?> GetByUserIdAsync(string userId);
        Task<IEnumerable<Student>> GetAllAsync();
        Task<int> AddAsync(Student student);
        Task UpdateAsync(Student student);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<int> GetCountAsync();
    }
}
