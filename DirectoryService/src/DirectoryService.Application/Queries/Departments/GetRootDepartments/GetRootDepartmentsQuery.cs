using SharedService.Core.Abstractions;

namespace DirectoryService.Application.Queries.Departments.GetRootDepartments;

public record GetRootDepartmentsQuery(
    int Page,
    int Size,
    int Prefetch) : IQuery;