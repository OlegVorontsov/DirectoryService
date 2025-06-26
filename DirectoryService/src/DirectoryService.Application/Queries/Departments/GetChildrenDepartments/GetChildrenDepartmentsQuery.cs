using SharedService.Core.Abstractions;

namespace DirectoryService.Application.Queries.Departments.GetChildrenDepartments;

public record GetChildrenDepartmentsQuery(
    Guid ParentId,
    int Page,
    int Size) : IQuery;