using DirectoryService.Application.Shared.DTOs;

namespace DirectoryService.Application.Queries.Departments.GetTopPositions;

public record GetTopPositionsResponse
{
    public required List<DepartmentWithPositionsDto> DepartmentWithPositions { get; set; }
}