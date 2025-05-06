using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using UniNest.DAL.Data;

namespace UniNest.Pages // Add this namespace declaration
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;

        public IndexModel(AppDbContext context)
        {
            _context = context;
        }

        public bool IsDatabaseConnected { get; private set; }

        public async Task OnGetAsync()
        {
            try
            {
                await _context.Database.CanConnectAsync();
                IsDatabaseConnected = true;
            }
            catch
            {
                IsDatabaseConnected = false;
            }
        }
    }
}