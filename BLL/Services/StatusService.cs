using BLL.Interfaces;
using DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class StatusService : IStatusService
    {
        private readonly IStatusRepository _repo;

        public StatusService(IStatusRepository repo)
        {
            _repo = repo;
        }

        public Task<string?> GetNameAsync(int statusId) => _repo.GetNameByIdAsync(statusId);

        public Task<int?> GetIdAsync(string name) => _repo.GetIdByNameAsync(name);
    }

}
