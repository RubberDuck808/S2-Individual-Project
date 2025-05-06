using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UniNest.DAL.Data;
using UniNest.DAL.Repositories;
using UniNest.DAL.Entities;
using UniNest.BLL.MappingProfiles;
using AutoMapper;
using UniNest.DAL.Interfaces;
using UniNest.BLL.Interfaces;
using UniNest.BLL.Services;


var builder = WebApplication.CreateBuilder(args);

// Add Razor Pages
builder.Services.AddRazorPages();

// Add AutoMapper with all profiles from the BLL assembly
builder.Services.AddAutoMapper(typeof(LandlordProfile).Assembly);

// Register DbContext with SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure Identity
builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
})
.AddEntityFrameworkStores<AppDbContext>();

// Register generic repository base
builder.Services.AddScoped(typeof(IRepository<>), typeof(BaseRepository<>));

// Register repositories (DAL layer)
builder.Services.AddScoped<ILandlordRepository, LandlordRepository>();
builder.Services.AddScoped<IAccommodationRepository, AccommodationRepository>();
builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<IApplicationRepository, ApplicationRepository>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();


// Register services (BLL layer)
builder.Services.AddScoped<ILandlordService, LandlordService>();
builder.Services.AddScoped<IAccommodationService, AccommodationService>();
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<IApplicationService, ApplicationService>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IUniversityService, UniversityService>();

var app = builder.Build();

// Seed sample data in development
if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        try
        {
            await SeedData.Initialize(services);
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred while seeding the database.");
        }
    }
}

// Configure HTTP pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
