using SharedService.SharedKernel.BaseClasses;

namespace DirectoryService.Domain.Models;

public class DepartmentPosition
    : IComparable<DepartmentPosition>, IEquatable<DepartmentPosition>
{
    public Id<Department> DepartmentId { get; }
    public Department Department { get; }
    public Id<Position> PositionId { get; }
    public Position Position { get; }

    public DepartmentPosition(
        Id<Department> departmentId,
        Id<Position> positionId)
    {
        DepartmentId = departmentId;
        PositionId = positionId;
    }

    // EF Core
    private DepartmentPosition() {}

    public int CompareTo(DepartmentPosition? other)
    {
        if (other == null)
            return -1;

        if (DepartmentId.CompareTo(other.DepartmentId) == 0 && PositionId.CompareTo(other.PositionId) == 0)
            return 0;

        return -1;
    }

    public bool Equals(DepartmentPosition? other)
    {
        if (other is null)
            return false;
        return DepartmentId.Equals(other.DepartmentId) && PositionId.Equals(other.PositionId);
    }

    public override bool Equals(object? obj)
    {
        if (obj is DepartmentPosition other)
            return Equals(other);
        return false;
    }
}