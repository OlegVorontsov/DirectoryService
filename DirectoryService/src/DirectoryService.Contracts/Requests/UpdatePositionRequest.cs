using DirectoryService.Application.Commands.Positions.UpdatePosition;

namespace DirectoryService.Contracts.Requests;

public record UpdatePositionRequest(
    string Name,
    string Description,
    IEnumerable<Guid> DepartmentIds)
{
    public UpdatePositionCommand ToCommand(Guid id) =>
        new (id, Name, Description, DepartmentIds);
}