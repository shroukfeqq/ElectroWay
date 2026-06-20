using ElctroWay.Models.Identity;
using ElctroWay.Models.booking;
using ElctroWay.Models.Provider;
using ElctroWay.Models.Payment;
using ElctroWay.Models.vehicle;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ElctroWay.Models.OurSystem;
using Microsoft.AspNetCore.Identity;

namespace ElctroWay.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser, IdentityRole<int>, int>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        // DbSets
        public DbSet<ProviderProfile> ProviderProfiles { get; set; }
        public DbSet<ProviderDocument> ProviderDocuments { get; set; }

        public DbSet<Station> Stations { get; set; }
        public DbSet<StationImage> StationImages { get; set; }

        public DbSet<Port> Ports { get; set; }
        public DbSet<PortImage> PortImages { get; set; }

        public DbSet<Booking> Bookings { get; set; }
        public DbSet<ChargingSession> ChargingSessions { get; set; }

        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Receipt> Receipts { get; set; }

        public DbSet<WithdrawalRequest> WithdrawalRequests { get; set; }

        public DbSet<Vehicle> Vehicles { get; set; }

        public DbSet<Favorite> Favorites { get; set; }
        public DbSet<Review> Reviews { get; set; }

        public DbSet<Notification> Notifications { get; set; }

        public DbSet<AdminAuditLog> AdminAuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // =========================
            // APPLICATION USER
            // =========================
            builder.Entity<ApplicationUser>()
                .HasOne(u => u.ProviderProfile)
                .WithOne(p => p.User)
                .HasForeignKey<ProviderProfile>(p => p.UserId);

            // =========================
            // BOOKING
            // =========================
            builder.Entity<Booking>()
                .HasOne(b => b.User)
                .WithMany(u => u.Bookings)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Booking>()
                .HasOne(b => b.Port)
                .WithMany(p => p.Bookings)
                .HasForeignKey(b => b.PortId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Booking>()
                .HasOne(b => b.ChargingSession)
                .WithOne(s => s.Booking)
                .HasForeignKey<ChargingSession>(s => s.BookingId);

            // =========================
            // CHARGING SESSION → TRANSACTION (1:1)
            // =========================
            builder.Entity<ChargingSession>()
                .HasOne(s => s.Transaction)
                .WithOne(t => t.Session)
                .HasForeignKey<Transaction>(t => t.SessionId);

            // =========================
            // FAVORITES (Unique Constraint)
            // =========================
            builder.Entity<Favorite>()
                .HasIndex(f => new { f.UserId, f.StationId })
                .IsUnique();


             builder.Entity<Favorite>()
            .HasOne(f => f.User)
            .WithMany(u => u.Favorites)
            .HasForeignKey(f => f.UserId)
            .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Favorite>()
                .HasOne(f => f.Station)
                .WithMany(s => s.Favorites)
                .HasForeignKey(f => f.StationId)
                .OnDelete(DeleteBehavior.Restrict);
            // =========================
            // REVIEWS (Optional: prevent duplicates)
            // =========================
            builder.Entity<Review>()
                .HasIndex(r => new { r.UserId, r.SessionId })
                .IsUnique();

            // =========================
            // TRANSACTION
            // =========================
            builder.Entity<Transaction>()
                .HasIndex(t => t.TxCode)
                .IsUnique();

            // =========================
            // STATION → PROVIDER
            // =========================
            builder.Entity<Station>()
                .HasOne(s => s.Provider)
                .WithMany(p => p.Stations)
                .HasForeignKey(s => s.ProviderId);

            // =========================
            // PROVIDER DOCUMENT
            // =========================
            builder.Entity<ProviderDocument>()
                .HasOne(d => d.Provider)
                .WithMany(p => p.Documents)
                .HasForeignKey(d => d.ProviderId);

            // =========================
            // PORT → STATION
            // =========================
            builder.Entity<Port>()
                .HasOne(p => p.Station)
                .WithMany(s => s.Ports)
                .HasForeignKey(p => p.StationId);

            // =========================
            // VEHICLE
            // =========================
            builder.Entity<Vehicle>()
                .HasOne(v => v.User)
                .WithMany(u => u.Vehicles)
                .HasForeignKey(v => v.UserId);

            // =========================
            // NOTIFICATION
            // =========================
            builder.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(n => n.UserId);

            // =========================
            // ADMIN AUDIT LOG
            // =========================
            builder.Entity<AdminAuditLog>()
                .HasOne(a => a.Admin)
                .WithMany()
                .HasForeignKey(a => a.AdminId)
                .OnDelete(DeleteBehavior.Restrict);
            //DECIMAL PRECISION FIX (ONLY ADDITION)
            // ==================================================

            builder.Entity<Receipt>()
                .Property(x => x.EnergyKwh)
                .HasPrecision(18, 4);

            builder.Entity<Receipt>()
                .Property(x => x.PricePerKwh)
                .HasPrecision(18, 4);

            builder.Entity<Receipt>()
                .Property(x => x.TotalAmount)
                .HasPrecision(18, 2);

            builder.Entity<Transaction>()
                .Property(x => x.Amount)
                .HasPrecision(18, 2);

            builder.Entity<WithdrawalRequest>()
                .Property(x => x.Amount)
                .HasPrecision(18, 2);

            builder.Entity<ChargingSession>()
                .Property(x => x.EnergyKwh)
                .HasPrecision(18, 4);

            builder.Entity<ChargingSession>()
                .Property(x => x.OwnerProfit)
                .HasPrecision(18, 2);

            builder.Entity<ChargingSession>()
                .Property(x => x.PlatformFee)
                .HasPrecision(18, 2);

            builder.Entity<ChargingSession>()
                .Property(x => x.TotalAmount)
                .HasPrecision(18, 2);

            builder.Entity<Port>()
                .Property(x => x.PricePerKwh)
                .HasPrecision(18, 4);

            builder.Entity<Vehicle>()
                .Property(x => x.BatteryCapacity)
                .HasPrecision(18, 2);

            builder.Entity<Vehicle>()
                .Property(x => x.ConsumptionRate)
                .HasPrecision(18, 4);
        }
    }
}