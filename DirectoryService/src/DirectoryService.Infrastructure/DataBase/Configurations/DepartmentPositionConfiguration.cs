using DirectoryService.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedService.SharedKernel.BaseClasses;

namespace DirectoryService.Infrastructure.DataBase.Configurations;

public class DepartmentPositionConfiguration : IEntityTypeConfiguration<DepartmentPosition>
{
    public void Configure(EntityTypeBuilder<DepartmentPosition> builder)
    {
        builder.ToTable("department_positions");

        builder.HasKey(d => new { d.DepartmentId, d.PositionId });

        builder.Property(d => d.DepartmentId)
            .HasConversion(
                id => id.Value,
                value => Id<Department>.Create(value))
            .HasColumnName("department_id");

        builder.Property(d => d.PositionId)
            .HasConversion(
                id => id.Value,
                value => Id<Position>.Create(value))
            .HasColumnName("position_id");

        builder.HasOne(dl => dl.Department)
            .WithMany(d => d.DepartmentPositions)
            .HasForeignKey(dl => dl.DepartmentId);

        builder.HasOne(dl => dl.Position)
            .WithMany(l => l.DepartmentPositions)
            .HasForeignKey(dl => dl.PositionId);
    }
}