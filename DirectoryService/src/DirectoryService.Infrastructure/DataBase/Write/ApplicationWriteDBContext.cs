using DirectoryService.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace DirectoryService.Infrastructure.DataBase.Write;

public class ApplicationWriteDBContext : DbContext
{
    public const string DATABASE_CONFIGURATION = "Database";

    private readonly string _connectionString;

    public ApplicationWriteDBContext(
        string connectionString)
    {
        _connectionString = connectionString;
    }

    public DbSet<Department> Departments { get; set; }
    public DbSet<DepartmentLocation> DepartmentLocations { get; set; }
    public DbSet<DepartmentPosition> DepartmentPositions { get; set; }
    public DbSet<Location> Locations { get; set; }
    public DbSet<Position> Positions { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(_connectionString);
        optionsBuilder.LogTo(Console.WriteLine)
                      .EnableSensitiveDataLogging();
        optionsBuilder.UseSnakeCaseNamingConvention();

        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("directory_service");

        modelBuilder.HasPostgresExtension("ltree");  // used in entities: Department

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(ApplicationWriteDBContext).Assembly,
            type => type.FullName?.Contains("DataBase.Configurations") ?? false);

        base.OnModelCreating(modelBuilder);
    }
}