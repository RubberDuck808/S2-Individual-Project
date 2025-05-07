using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using DAL.Entities;

namespace DAL.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        // DbSets for all entities
        public DbSet<Student> Students { get; set; }
        public DbSet<Landlord> Landlords { get; set; }
        public DbSet<Accommodation> Accommodations { get; set; }
        public DbSet<Application> Applications { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<University> Universities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); 

            modelBuilder.Entity<Landlord>(entity =>
            {
                entity.HasMany(l => l.Accommodations)
                    .WithOne(a => a.Landlord)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<AccommodationAmenity>()
                .HasKey(aa => new { aa.AccommodationId, aa.AmenityId });
        }
    }
}