using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyApp.Core.Entities;
using MyApp.Infrastructure.Identity;

namespace MyApp.Infrastructure.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser, IdentityRole<int>, int>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        // Identity entities sudah otomatis dari IdentityDbContext
        // AspNetUsers, AspNetRoles, AspNetUserClaims, AspNetUserRoles, AspNetRoleClaims, AspNetUserLogins, AspNetUserTokens

        public DbSet<FileUpload> Files { get; set; } = default!;
        public DbSet<Position> Positions { get; set; } = default!;
        public DbSet<Role> Roles { get; set; } = default!;
        public DbSet<UserType> UserTypes { get; set; } = default!;
        public DbSet<Rank> Ranks { get; set; } = default!;
        public DbSet<Employee> Employees { get; set; } = default!;
        public DbSet<Building> Buildings { get; set; } = default!;
        public DbSet<Room> Rooms { get; set; } = default!;
        public DbSet<RoomCategory> RoomCategories { get; set; } = default!;
        public DbSet<RoomStatus> RoomStatus { get; set; } = default!;
        public DbSet<RoomCondition> RoomConditions { get; set; } = default!;
        public DbSet<Occupant> Occupants { get; set; } = default!;
        public DbSet<OccupantHistory> OccupantHistories { get; set; } = default!;
        public DbSet<Visitor> Visitors { get; set; } = default!;
        public DbSet<Vendor> Vendors { get; set; } = default!;
        public DbSet<MaintenanceRequest> MaintenanceRequests { get; set; } = default!;
        public DbSet<InventoryType> InventoryTypes { get; set; } = default!;
        public DbSet<Repository> Repositories { get; set; } = default!;
        public DbSet<Inventory> Inventories { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // Penting! untuk Identity configuration

            // Atur kolom binary agar bisa menampung file besar
            modelBuilder.Entity<Occupant>()
                .Property(o => o.DocumentData)
                .HasColumnType("LONGBLOB");

            modelBuilder.Entity<Occupant>()
                .Property(o => o.PhotoData)
                .HasColumnType("LONGBLOB");

            // Pastikan semua Id auto increment
            modelBuilder.Entity<FileUpload>().Property(f => f.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<Position>().Property(p => p.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<Role>().Property(r => r.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<UserType>().Property(t => t.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<Rank>().Property(r => r.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<Employee>().Property(e => e.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<Building>().Property(b => b.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<Room>().Property(r => r.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<RoomCategory>().Property(rc => rc.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<RoomStatus>().Property(rs => rs.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<RoomCondition>().Property(rc => rc.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<Occupant>().Property(o => o.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<OccupantHistory>().Property(h => h.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<Visitor>().Property(v => v.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<Vendor>().Property(v => v.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<MaintenanceRequest>().Property(v => v.Id).ValueGeneratedOnAdd();
        }
    }
}
