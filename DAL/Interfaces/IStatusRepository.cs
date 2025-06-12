using DAL.Models;

namespace DAL.Interfaces
{
    public interface IStatusRepository
    {
        Task<string?> GetNameByIdAsync(int statusId);
        Task<int?> GetIdByNameAsync(string name);
        Task<List<Status>> GetAllAsync();
    }
}
