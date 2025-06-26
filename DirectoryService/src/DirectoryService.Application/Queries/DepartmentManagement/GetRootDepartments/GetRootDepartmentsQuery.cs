using SharedService.Core.Abstractions;

namespace DirectoryService.Application.Queries.DepartmentManagement.GetRootDepartments;

public record GetRootDepartmentsQuery(
    int Page,
    int Size,
    int Prefetch) : IQuery;