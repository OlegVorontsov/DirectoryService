using SharedService.Core.Abstractions;

namespace DirectoryService.Application.Commands.PositionManagement.CreatePosition;

public record CreatePositionCommand(
    string Name,
    string Description,
    IEnumerable<Guid> DepartmentIds) : ICommand;