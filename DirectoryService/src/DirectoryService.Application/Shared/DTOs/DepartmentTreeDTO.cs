namespace DirectoryService.Application.Shared.DTOs;

public record DepartmentTreeDTO
{
    public Guid Id { get; init; }
    public string Name { get; init; } = null!;
    public string Path { get; init; } = null!;
    public short Depth { get; init; }
    public Guid? ParentId { get; init; } = null!;
    public int ChildrenCount { get; init; }
    public List<DepartmentTreeDTO> Children { get; init; } = [];
}