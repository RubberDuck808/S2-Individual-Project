using Microsoft.EntityFrameworkCore;
using DAL.Data;
using DAL.Entities;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;


public static class SeedData
{
    public static async Task Initialize(IServiceProvider serviceProvider)
    {
        using var context = new AppDbContext(
            serviceProvider.GetRequiredService<DbContextOptions<AppDbContext>>());

        if (!context.Universities.Any())
        {
            context.Universities.AddRange(
                new University { Name = "University of London", Location = "London" },
                new University { Name = "University of Manchester", Location = "Manchester" }
            );
            await context.SaveChangesAsync();
        }
    }
}