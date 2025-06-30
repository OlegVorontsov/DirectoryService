using SharedService.Core.Abstractions;

namespace DirectoryService.Application.Commands.Departments.SoftDeleteDepartment;

public record SoftDeleteDepartmentCommand(
    Guid Id) : ICommand;