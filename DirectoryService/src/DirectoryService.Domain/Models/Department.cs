using CSharpFunctionalExtensions;
using DirectoryService.Domain.Shared.BaseClasses;
using DirectoryService.Domain.ValueObjects.DepartmentValueObjects;
using Microsoft.EntityFrameworkCore;
using SharedService.SharedKernel.Errors;

namespace DirectoryService.Domain.Models;

public class Department
{
    public Id<Department> Id { get; }
    public DepartmentName Name { get; }
    public Id<Department>? ParentId { get; }
    public Department? Parent { get; }
    public LTree Path { get; }
    public short Depth { get; }
    public int ChildrenCount { get; }
    public bool IsActive { get; } = true;
    public DateTime CreatedAt { get; }
    public DateTime UpdatedAt { get; }

    private readonly List<DepartmentLocation> _departmentLocations = [];
    public IReadOnlyList<DepartmentLocation> DepartmentLocations => _departmentLocations.ToList();

    public static Result<Department, Error> Create(
        Id<Department> id,
        DepartmentName name,
        string path,
        short depth,
        int childrenCount,
        DateTime createdAt)
    {
        if (string.IsNullOrEmpty(path))
            return Errors.General.ValueIsInvalid(nameof(Path));

        if (childrenCount < 0)
            return Errors.General.ValueIsInvalid(nameof(ChildrenCount));

        if (depth < 0)
            return Errors.General.ValueIsInvalid(nameof(Depth));

        return new Department(
            id,
            name,
            path,
            depth,
            childrenCount,
            createdAt);
    }

    // EF Core
    private Department() { }

    private Department(
        Id<Department> id,
        DepartmentName name,
        string path,
        short depth,
        int childrenCount,
        DateTime createdAt)
    {
        Id = id;
        Name = name;
        Path = path;
        Depth = depth;
        ChildrenCount = childrenCount;
        CreatedAt = createdAt;
        UpdatedAt = CreatedAt;
    }
}