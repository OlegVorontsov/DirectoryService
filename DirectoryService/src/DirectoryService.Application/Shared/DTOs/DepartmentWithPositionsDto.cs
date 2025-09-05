namespace DirectoryService.Application.Shared.DTOs;

public record DepartmentWithPositionsDto
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public string Path { get; init; }
    public short Depth { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
    public long PositionsCount { get; init; }
}