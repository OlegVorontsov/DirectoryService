using SharedService.Core.Abstractions;

namespace DirectoryService.Application.Commands.Departments.MoveDepartment;

public record MoveDepartmentCommand(
    Guid DepartmentId,
    Guid? NewParentDepartmentId) : ICommand;