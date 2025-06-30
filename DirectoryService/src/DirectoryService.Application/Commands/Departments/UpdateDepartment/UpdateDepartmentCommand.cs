using SharedService.Core.Abstractions;

namespace DirectoryService.Application.Commands.Departments.UpdateDepartment;

public record UpdateDepartmentCommand(
    Guid Id,
    string Name,
    IEnumerable<Guid> LocationIds,
    Guid? ParentId = null) : ICommand;