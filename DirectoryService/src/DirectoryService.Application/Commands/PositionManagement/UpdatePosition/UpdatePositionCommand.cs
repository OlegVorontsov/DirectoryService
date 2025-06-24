using SharedService.Core.Abstractions;

namespace DirectoryService.Application.Commands.PositionManagement.UpdatePosition;

public record UpdatePositionCommand(
    Guid Id,
    string Name,
    string Description,
    IEnumerable<Guid> DepartmentIds) : ICommand;