using DirectoryService.Application.Commands.DepartmentManagement.UpdateDepartment;

namespace DirectoryService.Presentation.Requests;

public record UpdateDepartmentRequest(
    string Name,
    IEnumerable<Guid> LocationIds,
    Guid? ParentId = null)
{
    public UpdateDepartmentCommand ToCommand(Guid id) =>
        new(id, Name, LocationIds, ParentId);
}