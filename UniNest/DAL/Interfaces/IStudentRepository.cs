using UniNest.DAL.Entities;

namespace UniNest.DAL.Interfaces
{
    public interface IStudentRepository : IRepository<Student>
    {
        Task<Student?> GetByUserIdAsync(string userId);
        Task<int> GetCountAsync();
    }
}
