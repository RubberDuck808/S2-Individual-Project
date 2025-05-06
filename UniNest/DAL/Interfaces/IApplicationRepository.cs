using UniNest.DAL.Entities;

namespace UniNest.DAL.Interfaces
{
    public interface IApplicationRepository : IRepository<Application>
    {
        Task<IEnumerable<Application>> GetByStudentAsync(int studentId);
        Task<IEnumerable<Application>> GetByLandlordIdAsync(int landlordId);
    }
}
