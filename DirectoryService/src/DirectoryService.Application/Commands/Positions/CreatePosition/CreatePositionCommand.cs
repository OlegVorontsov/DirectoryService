using SharedService.Core.Abstractions;

namespace DirectoryService.Application.Commands.Positions.CreatePosition;

public record CreatePositionCommand(
    string Name,
    string Description,
    IEnumerable<Guid> DepartmentIds) : ICommand;