using DirectoryService.Domain.Models;
using DirectoryService.Domain.Shared.BaseClasses;
using DirectoryService.Domain.ValueObjects.LocationValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DirectoryService.Infrastructure.DataBase.Configurations;

public class LocationConfiguration : IEntityTypeConfiguration<Location>
{
    public void Configure(EntityTypeBuilder<Location> builder)
    {
        builder.ToTable("locations");

        builder.HasKey(d => d.Id);
        builder.Property(d => d.Id)
               .HasConversion(
                   id => id.Value,
                   value => Id<Location>.Create(value))
               .HasColumnName("id");

        builder.Property(d => d.IsActive)
               .IsRequired()
               .HasColumnName("is_active");
        builder.HasQueryFilter(d => d.IsActive); // global query filter

        builder.Property(d => d.Name)
               .HasConversion(
                   name => name.Value,
                   value => LocationName.Create(value).Value!)
               .IsRequired()
               .HasMaxLength(LocationName.MAX_LENGTH)
               .HasColumnName("name");

        builder.HasIndex(d => d.Name)
               .IsUnique();

        builder.OwnsOne(d => d.Address, ib =>
        {
            ib.ToJson("address");

            ib.Property(i => i.City)
                .IsRequired()
                .HasMaxLength(LocationAddress.CITY_MAX_LENGTH);

            ib.Property(i => i.Street)
                .IsRequired()
                .HasMaxLength(LocationAddress.STREET_MAX_LENGTH);

            ib.Property(i => i.HouseNumber)
                .IsRequired()
                .HasMaxLength(LocationAddress.HOUSENUMBER_MAX_LENGTH);
        });

        builder.ComplexProperty(d => d.TimeZone, ib =>
        {
            ib.Property(i => i.Value)
                .IsRequired()
                .HasColumnName("time_zone");
        });

        builder.HasMany(l => l.DepartmentLocations)
               .WithOne(dl => dl.Location)
               .HasForeignKey(dl => dl.LocationId)
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