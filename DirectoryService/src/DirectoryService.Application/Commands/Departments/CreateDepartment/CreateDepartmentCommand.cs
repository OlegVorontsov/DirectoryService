using SharedService.Core.Abstractions;

namespace DirectoryService.Application.Commands.Departments.CreateDepartment;

public record CreateDepartmentCommand(
    string Name,
    IEnumerable<Guid> LocationIds,
    Guid? ParentId = null) : ICommand;