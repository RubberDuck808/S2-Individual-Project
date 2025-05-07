using DAL.Entities;

namespace DAL.Interfaces
{
    public interface IStudentRepository : IRepository<Student>
    {
        Task<Student?> GetByUserIdAsync(string userId);
        Task<int> GetCountAsync();
    }
}
