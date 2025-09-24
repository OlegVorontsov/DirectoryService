using DirectoryService.Domain.Models;

namespace DirectoryService.Application.Shared.DTOs;

public record DepartmentDTO(
    Guid Id,
    string Name,
    Guid? ParentId,
    string Path,
    short Depth,
    int ChildrenCount,
    bool IsActive,
    DateTime CreatedAt,
    DateTime UpdatedAt)
{
    public static DepartmentDTO FromDomainEntity(Department entity)
        => new(
            Id: entity.Id.Value,
            Name: entity.Name.Value,
            ParentId: entity.ParentId?.Value,
            Path: entity.Path,
            Depth: entity.Depth,
            ChildrenCount: entity.ChildrenCount,
            IsActive: entity.IsActive,
            CreatedAt: entity.CreatedAt,
            UpdatedAt: entity.UpdatedAt);
}