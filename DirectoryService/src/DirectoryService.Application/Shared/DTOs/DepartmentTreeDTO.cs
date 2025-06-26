using DirectoryService.Domain.Models;

namespace DirectoryService.Application.Shared.DTOs;

public class DepartmentTreeDTO
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Path { get; private set; }
    public short Depth { get; private set; }
    public Guid? ParentId { get; private set; }
    public List<DepartmentTreeDTO> Children { get; private set; } = [];
    public int ChildrenCount { get; private set; }

    public DepartmentTreeDTO()
    {
    }

    public static DepartmentTreeDTO FromDomainEntity(Department entity, IEnumerable<Department>? children = null)
        => new DepartmentTreeDTO
        {
            //entity.Id.Value,
            //entity.Name.Value,
            //entity.Path,
            //entity.Depth,
            //entity.ChildrenCount
            Children = children is null ? [] : children.Select(d => FromDomainEntity(d)).ToList(),
        };
}