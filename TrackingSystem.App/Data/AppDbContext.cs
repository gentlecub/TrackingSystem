using Microsoft.EntityFrameworkCore;
using TrackingSystem.App.Models;
using TrackingSystem.App.Models.Enums;

namespace TrackingSystem.App.Data;

public class AppDbContext : DbContext
{
    public DbSet<Asset> Assets { get; set; }
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Office> Offices { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<MaintenanceRecord> MaintenanceRecords { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public AppDbContext()
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            // SQL Server configuration (Docker)
            optionsBuilder.UseSqlServer(
                "Server=localhost,1433;Database=AssetTrackingDB;User Id=sa;Password=Cyber*1991;TrustServerCertificate=True;"
            );
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure Asset
        modelBuilder.Entity<Asset>(entity =>
        {
            entity.HasIndex(a => a.SerialNumber).IsUnique();

            entity.Property(a => a.PurchasePriceUSD)
                .HasPrecision(18, 2);

            entity.Property(a => a.LocalPrice)
                .HasPrecision(18, 2);

            // An Asset belongs to one Office
            entity.HasOne(a => a.Office)
                .WithMany(o => o.Assets)
                .HasForeignKey(a => a.OfficeId)
                .OnDelete(DeleteBehavior.Restrict);

            // An Asset can be assigned to an Employee (optional)
            entity.HasOne(a => a.Employee)
                .WithMany(e => e.AssignedAssets)
                .HasForeignKey(a => a.EmployeeId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Configure Office
        modelBuilder.Entity<Office>(entity =>
        {
            entity.Property(o => o.ExchangeRate)
                .HasPrecision(18, 4);
        });

        // Configure User
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(u => u.Username).IsUnique();

            entity.HasOne(u => u.Employee)
                .WithOne(e => e.User)
                .HasForeignKey<User>(u => u.EmployeeId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Configure MaintenanceRecord
        modelBuilder.Entity<MaintenanceRecord>(entity =>
        {
            entity.HasOne(m => m.Asset)
                .WithMany(a => a.MaintenanceRecords)
                .HasForeignKey(m => m.AssetId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Seed Data
        SeedData(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        // Offices
        modelBuilder.Entity<Office>().HasData(
            new Office
            {
                Id = 1,
                Name = "Stockholm HQ",
                Country = "Sweden",
                City = "Stockholm",
                CurrencyCode = "SEK",
                ExchangeRate = 10.5m
            },
            new Office
            {
                Id = 2,
                Name = "London Office",
                Country = "UK",
                City = "London",
                CurrencyCode = "GBP",
                ExchangeRate = 0.79m
            },
            new Office
            {
                Id = 3,
                Name = "Madrid Office",
                Country = "Spain",
                City = "Madrid",
                CurrencyCode = "EUR",
                ExchangeRate = 0.92m
            }
        );

        // Employees
        modelBuilder.Entity<Employee>().HasData(
            new Employee
            {
                Id = 1,
                FullName = "Anna Andersson",
                Department = "IT",
                Email = "anna.andersson@company.com"
            },
            new Employee
            {
                Id = 2,
                FullName = "Erik Eriksson",
                Department = "Finance",
                Email = "erik.eriksson@company.com"
            },
            new Employee
            {
                Id = 3,
                FullName = "Maria Garcia",
                Department = "HR",
                Email = "maria.garcia@company.com"
            },
            new Employee
            {
                Id = 4,
                FullName = "John Smith",
                Department = "Sales",
                Email = "john.smith@company.com"
            }
        );

        // Users
        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = 1,
                Username = "admin",
                Password = "admin123",
                Role = UserRole.Admin,
                EmployeeId = null
            },
            new User
            {
                Id = 2,
                Username = "manager",
                Password = "manager123",
                Role = UserRole.Manager,
                EmployeeId = 1
            },
            new User
            {
                Id = 3,
                Username = "employee",
                Password = "employee123",
                Role = UserRole.Employee,
                EmployeeId = 2
            }
        );

        // Sample assets
        modelBuilder.Entity<Asset>().HasData(
            // Stockholm Office - Old assets (to show colors)
            new Asset
            {
                Id = 1,
                Type = AssetType.Laptop,
                Brand = "Dell",
                ModelName = "XPS 15",
                PurchaseDate = DateTime.Now.AddYears(-3).AddMonths(1), // Red - almost expiring
                PurchasePriceUSD = 1500.00m,
                LocalPrice = 15750.00m,
                SerialNumber = "DELL-XPS-001",
                WarrantyExpirationDate = DateTime.Now.AddMonths(-11),
                OfficeId = 1,
                EmployeeId = 1
            },
            new Asset
            {
                Id = 2,
                Type = AssetType.Laptop,
                Brand = "Apple",
                ModelName = "MacBook Pro 14",
                PurchaseDate = DateTime.Now.AddYears(-2).AddMonths(-8), // Yellow - warning
                PurchasePriceUSD = 2400.00m,
                LocalPrice = 25200.00m,
                SerialNumber = "APPLE-MBP-001",
                WarrantyExpirationDate = DateTime.Now.AddMonths(4),
                OfficeId = 1,
                EmployeeId = 2
            },
            new Asset
            {
                Id = 3,
                Type = AssetType.Desktop,
                Brand = "HP",
                ModelName = "EliteDesk 800",
                PurchaseDate = DateTime.Now.AddYears(-1), // Green - OK
                PurchasePriceUSD = 1200.00m,
                LocalPrice = 12600.00m,
                SerialNumber = "HP-ED-001",
                WarrantyExpirationDate = DateTime.Now.AddYears(2),
                OfficeId = 1,
                EmployeeId = null
            },
            // London Office
            new Asset
            {
                Id = 4,
                Type = AssetType.MobilePhone,
                Brand = "Apple",
                ModelName = "iPhone 15 Pro",
                PurchaseDate = DateTime.Now.AddMonths(-6), // Green
                PurchasePriceUSD = 1100.00m,
                LocalPrice = 869.00m,
                SerialNumber = "APPLE-IP15-001",
                WarrantyExpirationDate = DateTime.Now.AddMonths(18),
                OfficeId = 2,
                EmployeeId = 3
            },
            new Asset
            {
                Id = 5,
                Type = AssetType.Laptop,
                Brand = "Lenovo",
                ModelName = "ThinkPad X1 Carbon",
                PurchaseDate = DateTime.Now.AddYears(-2).AddMonths(-10), // Red
                PurchasePriceUSD = 1800.00m,
                LocalPrice = 1422.00m,
                SerialNumber = "LEN-X1C-001",
                WarrantyExpirationDate = DateTime.Now.AddMonths(-10),
                OfficeId = 2,
                EmployeeId = 4
            },
            // Madrid Office
            new Asset
            {
                Id = 6,
                Type = AssetType.Tablet,
                Brand = "Apple",
                ModelName = "iPad Pro 12.9",
                PurchaseDate = DateTime.Now.AddYears(-1).AddMonths(-6), // Green
                PurchasePriceUSD = 1100.00m,
                LocalPrice = 1012.00m,
                SerialNumber = "APPLE-IPAD-001",
                WarrantyExpirationDate = DateTime.Now.AddMonths(6),
                OfficeId = 3,
                EmployeeId = null
            },
            new Asset
            {
                Id = 7,
                Type = AssetType.OfficeEquipment,
                Brand = "HP",
                ModelName = "LaserJet Pro M428",
                PurchaseDate = DateTime.Now.AddYears(-2).AddMonths(-6), // Yellow
                PurchasePriceUSD = 450.00m,
                LocalPrice = 414.00m,
                SerialNumber = "HP-PRINT-001",
                WarrantyExpirationDate = DateTime.Now.AddMonths(-6),
                OfficeId = 3,
                EmployeeId = null
            }
        );
    }
}
