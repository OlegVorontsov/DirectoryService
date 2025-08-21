using CSharpFunctionalExtensions;
using DirectoryService.Domain.ValueObjects.Locations;
using SharedService.SharedKernel.BaseClasses;

namespace DirectoryService.Domain.Models;

public class Location
{
    public Id<Location> Id { get; private set; }
    public LocationName Name { get; private set; }
    public LocationAddress Address { get; private set; }
    public IANATimeZone TimeZone { get; private set; }
    public bool IsActive { get; private set; } = true;
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private List<DepartmentLocation> _departmentLocations = [];
    public IReadOnlyList<DepartmentLocation> DepartmentLocations => _departmentLocations.AsReadOnly();

    public static Result<Location> Create(
        Id<Location> id,
        LocationName name,
        LocationAddress address,
        IANATimeZone timeZone)
    {
        return new Location(
            id,
            name,
            address,
            timeZone);
    }

    public Location Update(
        LocationName name,
        LocationAddress address,
        IANATimeZone timeZone)
    {
        Name = name;
        Address = address;
        TimeZone = timeZone;
        UpdatedAt = DateTime.UtcNow;

        return this;
    }

    public Location Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
        return this;
    }

    public Location Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
        return this;
    }

    // EF Core
    private Location() { }

    private Location(
        Id<Location> id,
        LocationName name,
        LocationAddress address,
        IANATimeZone timeZone)
    {
        Id = id;
        Name = name;
        Address = address;
        TimeZone = timeZone;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
}