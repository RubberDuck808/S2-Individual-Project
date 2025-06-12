using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IStatusService
    {
        Task<string?> GetNameAsync(int statusId);
        Task<int?> GetIdAsync(string name);
    }

}
