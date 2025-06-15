using BLL.Interfaces;
using DAL.Interfaces;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class StatusService : IStatusService
    {
        private readonly IStatusRepository _repo;
        private readonly ILogger<StatusService> _logger;

        public StatusService(IStatusRepository repo, ILogger<StatusService> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        public async Task<string?> GetNameAsync(int statusId)
        {
            _logger.LogInformation("Fetching status name for ID: {StatusId}", statusId);
            var name = await _repo.GetNameByIdAsync(statusId);

            if (name == null)
                _logger.LogWarning("No status found for ID: {StatusId}", statusId);
            else
                _logger.LogInformation("Status ID {StatusId} corresponds to: {StatusName}", statusId, name);

            return name;
        }

        public async Task<int?> GetIdAsync(string name)
        {
            _logger.LogInformation("Fetching status ID for name: {StatusName}", name);
            var id = await _repo.GetIdByNameAsync(name);

            if (id == null)
                _logger.LogWarning("No status ID found for name: {StatusName}", name);
            else
                _logger.LogInformation("Status name {StatusName} corresponds to ID: {StatusId}", name, id);

            return id;
        }
    }
}
