using SharedService.Core.Abstractions;

namespace DirectoryService.Application.Commands.DepartmentManagement.SoftDeleteDepartment;

public record SoftDeleteDepartmentCommand(
    Guid Id) : ICommand;