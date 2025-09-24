using SharedService.Core.Abstractions;

namespace DirectoryService.Application.Queries.Departments.GetDepartmentById;

public record GetDepartmentByIdQuery(
    Guid Id) : IQuery;