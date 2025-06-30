using DirectoryService.Domain.Models;

namespace DirectoryService.Application.Shared.DTOs;

public record DepartmentTreeDTO(
    Guid Id,
    string Name,
    string Path,
    short Depth,
    Guid? ParentId,
    int ChildrenCount,
    List<DepartmentTreeDTO> Children)
{
    public static DepartmentTreeDTO FromDomainEntity(Department entity, IEnumerable<Department>? children = null)
        => new DepartmentTreeDTO(
            Id: entity.Id.Value,
            Name: entity.Name.Value,
            Path: entity.Path,
            Depth: entity.Depth,
            ParentId: entity.ParentId?.Value,
            ChildrenCount: entity.ChildrenCount,
            Children: children is null ? [] : children.Select(d => FromDomainEntity(d)).ToList());
}