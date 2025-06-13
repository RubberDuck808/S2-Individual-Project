using DAL.Models;

namespace DAL.Interfaces
{
    public interface IStudentRepository
    {
        Task<Student?> GetByUserIdAsync(string userId);
        Task<int> GetCountAsync();
    }
}
