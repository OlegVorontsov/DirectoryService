using DirectoryService.Application.Commands.Departments.MoveDepartment;
using DirectoryService.Application.Commands.Departments.UpdateDepartment;

namespace DirectoryService.Contracts.Requests;

public record MoveDepartmentRequest(
    Guid? ParentId = null)
{
    public MoveDepartmentCommand ToCommand(Guid id) =>
        new(id, ParentId);
}