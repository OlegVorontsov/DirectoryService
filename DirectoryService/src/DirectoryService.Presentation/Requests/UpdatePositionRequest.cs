using DirectoryService.Application.Commands.PositionManagement.UpdatePosition;

namespace DirectoryService.Presentation.Requests;

public record UpdatePositionRequest(
    string Name,
    string Description,
    IEnumerable<Guid> DepartmentIds)
{
    public UpdatePositionCommand ToCommand(Guid id) =>
        new (id, Name, Description, DepartmentIds);
}