using DirectoryService.Domain.Models;
using DirectoryService.Domain.Shared.BaseClasses;
using DirectoryService.Domain.ValueObjects.PositionValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DirectoryService.Infrastructure.DataBase.Configurations;

public class PositionConfiguration : IEntityTypeConfiguration<Position>
{
    public void Configure(EntityTypeBuilder<Position> builder)
    {
        builder.ToTable("positions");

        builder.HasKey(d => d.Id);
        builder.Property(d => d.Id)
               .HasConversion(
                   id => id.Value,
                   value => Id<Position>.Create(value))
               .HasColumnName("id");

        builder.Property(d => d.IsActive)
               .IsRequired()
               .HasColumnName("is_active");
        builder.HasQueryFilter(d => d.IsActive);  // global query filter

        builder.Property(d => d.Name)
               .HasConversion(
                   name => name.Value,
                   value => PositionName.Create(value).Value!)
               .IsRequired()
               .HasMaxLength(PositionName.MAX_LENGTH)
               .HasColumnName("name");

        builder.HasIndex(d => d.Name)
               .IsUnique();

        builder.Property(d => d.Description)
               .HasConversion(
                   description => description.Value,
                   value => PositionDescription.Create(value).Value!)
               .IsRequired(false)
               .HasMaxLength(PositionDescription.MAX_LENGTH)
               .HasColumnName("description");

        builder.HasMany(d => d.DepartmentPositions)
            .WithOne(dp => dp.Position)
            .HasForeignKey(dp => dp.PositionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(d => d.CreatedAt)
               .HasConversion(
                   src => src.Kind == DateTimeKind.Utc ? src : DateTime.SpecifyKind(src, DateTimeKind.Utc),
                   dst => dst.Kind == DateTimeKind.Utc ? dst : DateTime.SpecifyKind(dst, DateTimeKind.Utc))
               .IsRequired()
               .HasColumnName("created_at");

        builder.Property(d => d.UpdatedAt)
               .HasConversion(
                   src => src.Kind == DateTimeKind.Utc ? src : DateTime.SpecifyKind(src, DateTimeKind.Utc),
                   dst => dst.Kind == DateTimeKind.Utc ? dst : DateTime.SpecifyKind(dst, DateTimeKind.Utc))
               .IsRequired()
               .HasColumnName("updated_at");
    }
}