using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.FileProviders;
using BLL.Interfaces;
using BLL.Services;
using BLL.MappingProfiles;
using DAL.Models;
using DAL.Interfaces;
using DAL.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.Data.SqlClient;
using AutoMapper;
using System.IO;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// AutoMapper
builder.Services.AddAutoMapper(typeof(LandlordProfile).Assembly);

// Get env var
var connectionString = Environment.GetEnvironmentVariable("UNINEST_CONNECTION_STRING");

Console.WriteLine("DEBUG: UNINEST_CONNECTION_STRING =");
Console.WriteLine(connectionString ?? "[null]");

if (string.IsNullOrEmpty(connectionString))
{
    throw new Exception("UNINEST_CONNECTION_STRING is not set in environment variables.");
}


// --- Repositories (ADO.NET, pass connection string) ---
builder.Services.AddScoped<ILandlordRepository>(_ => new LandlordRepository(connectionString));
builder.Services.AddScoped<IAccommodationRepository>(_ => new AccommodationRepository(connectionString));
builder.Services.AddScoped<IStudentRepository>(_ => new StudentRepository(connectionString));
builder.Services.AddScoped<IAccountService, AccountService>();


// --- Services (BL layer stays the same) ---
builder.Services.AddScoped<ILandlordService, LandlordService>();
builder.Services.AddScoped<IAccommodationService, AccommodationService>();
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<IUserRepository>(_ => new UserRepository(connectionString));
builder.Services.AddScoped<IUniversityRepository>(_ => new UniversityRepository(connectionString));


//builder.Services.AddScoped<IUniversityService, UniversityService>();

builder.Services.AddAuthorization();
builder.Services.AddRazorPages(); // Required for app.MapRazorPages() to work



var app = builder.Build();


// HTTP pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();


app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
