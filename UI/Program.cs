using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.FileProviders;
using BLL.Interfaces;
using BLL.Services;
using BLL.MappingProfiles;
using Domain.Models;
using DAL.Interfaces;
using DAL.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.Data.SqlClient;
using AutoMapper;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using APIWrapper;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables(); 


// AutoMapper
builder.Services.AddAutoMapper(typeof(LandlordProfile).Assembly);

var connectionString = Environment.GetEnvironmentVariable("UNINEST_CONNECTION_STRING");
var googleApiKey = Environment.GetEnvironmentVariable("GOOGLE_API_KEY");

if (string.IsNullOrEmpty(connectionString))
    throw new Exception("UNINEST_CONNECTION_STRING is not set in environment variables.");

if (string.IsNullOrEmpty(googleApiKey))
    throw new Exception("GOOGLE_API_KEY is not set in environment variables.");




// Services
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<ILandlordService, LandlordService>();
builder.Services.AddScoped<IAccommodationService, AccommodationService>();
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IApplicationService, ApplicationService>();
builder.Services.AddScoped<IUniversityService, UniversityService>();
builder.Services.AddScoped<IAccommodationTypeService, AccommodationTypeService>();
builder.Services.AddScoped<IAmenityService, AmenityService>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IStatusService, StatusService>();
builder.Services.AddScoped<IAccommodationImageService, AccommodationImageService>();
builder.Services.AddScoped<IAccommodationAssemblerService, AccommodationAssemblerService>();
builder.Services.AddScoped<IGeoLocationService, GeoLocationService>();
builder.Services.AddHttpClient<GoogleMapsApiWrapper>();
builder.Services.AddScoped<IGoogleMapsApiWrapper, GoogleMapsApiWrapper>(); // <-- must exist

builder.Services.AddScoped<IPasswordHasher<object>, PasswordHasher<object>>();





// Repositories
builder.Services.AddScoped<IUserRepository>(_ => new UserRepository(connectionString));
builder.Services.AddScoped<IUniversityRepository>(_ => new UniversityRepository(connectionString));
builder.Services.AddScoped<IAmenityRepository>(_ => new AmenityRepository(connectionString));
builder.Services.AddScoped<IAccommodationTypeRepository>(_ => new AccommodationTypeRepository(connectionString));
builder.Services.AddScoped<IAccommodationImageRepository>(_ => new AccommodationImageRepository(connectionString));
builder.Services.AddScoped<ILandlordRepository>(_ => new LandlordRepository(connectionString));
builder.Services.AddScoped<IAccommodationRepository>(_ => new AccommodationRepository(connectionString));
builder.Services.AddScoped<IStudentRepository>(_ => new StudentRepository(connectionString));
builder.Services.AddScoped<IApplicationRepository>(_ => new ApplicationRepository(connectionString));
builder.Services.AddScoped<IBookingRepository>(_ => new BookingRepository(connectionString));
builder.Services.AddScoped<IStatusRepository>(_ => new StatusRepository(connectionString));

builder.Services.AddHttpClient<GoogleMapsApiWrapper>();


builder.Services.AddRazorPages();

// Cookie Authentication
builder.Services.AddAuthentication("UniNestAuth")
    .AddCookie("UniNestAuth", options =>
    {
        options.LoginPath = "/Login";
        options.AccessDeniedPath = "/AccessDenied";
        options.Cookie.Name = "UniNestAuthCookie";

        
        options.ExpireTimeSpan = TimeSpan.FromMinutes(15);
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization();

var app = builder.Build();

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


// Error handling for status codes
app.UseStatusCodePages(async context =>
{
    var response = context.HttpContext.Response;

    if (response.StatusCode == 404)
    {
        response.Redirect("/NotFound");
    }
    else if (response.StatusCode == 403)
    {
        response.Redirect("/AccessDenied");
    }
});

app.MapRazorPages();

app.Run();