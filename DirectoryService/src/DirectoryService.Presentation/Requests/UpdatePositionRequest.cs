using DirectoryService.Application.Commands.PositionManagement.UpdatePosition;

namespace DirectoryService.Presentation.Requests;

public record UpdatePositionRequest(
    string Name,
    string Description)
{
    public UpdatePositionCommand ToCommand(Guid id) =>
        new (id, Name, Description);
}