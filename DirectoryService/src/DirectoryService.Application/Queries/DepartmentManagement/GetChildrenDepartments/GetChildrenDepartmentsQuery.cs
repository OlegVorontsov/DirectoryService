using SharedService.Core.Abstractions;

namespace DirectoryService.Application.Queries.DepartmentManagement.GetChildrenDepartments;

public record GetChildrenDepartmentsQuery(
    Guid ParentId,
    int Page,
    int Size) : IQuery;