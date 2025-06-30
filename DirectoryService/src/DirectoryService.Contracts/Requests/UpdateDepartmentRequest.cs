using DirectoryService.Application.Commands.Departments.UpdateDepartment;

namespace DirectoryService.Contracts.Requests;

public record UpdateDepartmentRequest(
    string Name,
    IEnumerable<Guid> LocationIds,
    Guid? ParentId = null)
{
    public UpdateDepartmentCommand ToCommand(Guid id) =>
        new(id, Name, LocationIds, ParentId);
}