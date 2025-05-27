using DirectoryService.Domain.Shared.BaseClasses;

namespace DirectoryService.Domain.Models;

public class DepartmentLocation
{
    public Id<Department> DepartmentId { get; private set; }
    public Department Department { get; private set; }
    public Id<Location> LocationId { get; private set; }
    public Location Location { get; private set; }

    public DepartmentLocation(
        Id<Department> departmentId,
        Id<Location> locationId)
    {
        DepartmentId = departmentId;
        LocationId = locationId;
    }

    // EF Core
    private DepartmentLocation() {}
}