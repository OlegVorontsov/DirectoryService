using DirectoryService.Application.Shared.DTOs;

namespace DirectoryService.Application.Queries.Departments.GetChildrenDepartments;

public record GetChildrenDepartmentsDto(int TotalCount, IEnumerable<DepartmentDTO> Departments);