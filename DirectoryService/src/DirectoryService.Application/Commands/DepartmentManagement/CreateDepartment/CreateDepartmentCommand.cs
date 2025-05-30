using SharedService.Core.Abstractions;

namespace DirectoryService.Application.Commands.DepartmentManagement.CreateDepartment;

public record CreateDepartmentCommand(
    string Name,
    IEnumerable<Guid> LocationIds,
    Guid? ParentId = null) : ICommand;