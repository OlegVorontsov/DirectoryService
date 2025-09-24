using DirectoryService.Application.Shared.DTOs;

namespace DirectoryService.Application.Queries.Departments.GetRootDepartments;

public record GetRootDepartmentsDto(int TotalCount, List<DepartmentTreeDTO> Departments);