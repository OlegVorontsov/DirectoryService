using DirectoryService.Domain.Models;
using DirectoryService.Domain.Shared.BaseClasses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DirectoryService.Infrastructure.DataBase.Configurations;

public class DepartmentLocationConfiguration : IEntityTypeConfiguration<DepartmentLocation>
{
    public void Configure(EntityTypeBuilder<DepartmentLocation> builder)
    {
        builder.ToTable("department_locations");

        builder.HasKey(d => new { d.DepartmentId, d.LocationId });

        builder.Property(d => d.DepartmentId)
               .HasConversion(
                   id => id.Value,
                   value => Id<Department>.Create(value))
               .HasColumnName("department_id");

        builder.Property(d => d.LocationId)
               .HasConversion(
                   id => id.Value,
                   value => Id<Location>.Create(value))
               .HasColumnName("location_id");

        builder.HasOne(dl => dl.Department)
               .WithMany(d => d.DepartmentLocations)
               .HasForeignKey(dl => dl.DepartmentId);

        builder.HasOne(dl => dl.Location)
               .WithMany(l => l.DepartmentLocations)
               .HasForeignKey(dl => dl.LocationId);
    }
}