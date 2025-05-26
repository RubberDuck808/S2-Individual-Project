using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System;
using System.Threading.Tasks;

namespace UI.Pages
{
    public class IndexModel : PageModel
    {
        private readonly string _connectionString;

        public IndexModel(IConfiguration config)
        {
            _connectionString = config.GetValue<string>("UNINEST_CONNECTION_STRING")
                ?? throw new Exception("Connection string not found.");
        }

        public bool IsDatabaseConnected { get; private set; }

        public async Task OnGetAsync()
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();
                IsDatabaseConnected = true;
            }
            catch
            {
                IsDatabaseConnected = false;
            }
        }
    }
}
