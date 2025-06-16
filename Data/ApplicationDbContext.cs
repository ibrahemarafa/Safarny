using APIs_Graduation.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace APIs_Graduation.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<BookingCustomTripDetail>()
           .HasOne(d => d.BookingCustomTrip)
           .WithMany(p => p.BookingCustomTripDetail)
           .HasForeignKey(d => d.BookingId)
           .OnDelete(DeleteBehavior.Cascade); // Main relation - Cascade

            modelBuilder.Entity<BookingCustomTripDetail>()
                .HasOne(d => d.City)
                .WithMany()
                .HasForeignKey(d => d.CityId)
                .OnDelete(DeleteBehavior.Restrict); // Restrict

            modelBuilder.Entity<BookingCustomTripDetail>()
                .HasOne(d => d.Hotel)
                .WithMany()
                .HasForeignKey(d => d.HotelId)
                .OnDelete(DeleteBehavior.Restrict); // Restrict

            modelBuilder.Entity<Trip_Place>()
                    .HasKey(tp => new { tp.TripId, tp.PlaceId });

            modelBuilder.Entity<Trip_Place>()
                .HasOne(tp => tp.Trip)
                .WithMany(t => t.TripPlaces)
                .HasForeignKey(tp => tp.TripId);

            modelBuilder.Entity<Trip_Place>()
                .HasOne(tp => tp.Place)
                .WithMany(p => p.TripPlaces)
                .HasForeignKey(tp => tp.PlaceId);

            modelBuilder.Entity<Package>()
                .Property(p => p.Price)
                .HasColumnType("decimal(18, 2)");

            modelBuilder.Entity<PackagePlan>()
                .HasOne(pp => pp.Package)
                .WithMany(p => p.PackagePlans)
                .HasForeignKey(pp => pp.PackageId);

            modelBuilder.Entity<PackageInclusion>()
                .HasOne(pi => pi.Package)
                .WithMany(p => p.PackageInclusions)
                .HasForeignKey(pi => pi.PackageId);

            modelBuilder.Entity<HotelBooking>()
              .HasOne(hb => hb.Room)
                .WithMany()
                .HasForeignKey(hb => hb.RoomId)
               .OnDelete(DeleteBehavior.NoAction);
        }

        public DbSet<City> Cities { get; set; }
        public DbSet<TouristPlaces> TouristPlaces { get; set; }
        public DbSet<Activity> Activities { get; set; }



        public DbSet<Package> Packages { get; set; }
        public DbSet<PackagePlan> PackagePlans { get; set; }
        public DbSet<PackageInclusion> PackageInclusions { get; set; }
        public DbSet<PackageExclusion> PackageExclusions { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<UserPreference> UserPreferences { get; set; }
        public DbSet<Package_Images> Package_Images { get; set; }
        public DbSet<PackageBooking> PackageBookings { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<HotelBooking> HotelBookings { get; set; }
        public DbSet<BookingCustomTrip> BookingCustomTrips { get; set; }
        public DbSet<BookingCustomTripDetail> BookingCustomTripDetails { get; set; }



        public DbSet<Hotel> Hotels { get; set; }
        public DbSet<Restaurant> restaurants { get; set; }
        public DbSet<Trip> Trips { get; set; }
        public DbSet<Trip_Place> Trip_Places { get; set; }
        public DbSet<Hotel_Image> Hotel_Images { get; set; }
        public DbSet<Hotel_Feature> Hotel_Features { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Hotel_Room_Feature> hotel_Room_Features { get; set; }
        public DbSet<UserInteraction> UserInteraction { get; set; }

        public DbSet<Cart> Carts { get; set; }


    }
}
