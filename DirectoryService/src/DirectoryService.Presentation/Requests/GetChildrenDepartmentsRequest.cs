using DirectoryService.Application.Queries.DepartmentManagement.GetChildrenDepartments;

namespace DirectoryService.Presentation.Requests;

public record GetChildrenDepartmentsRequest(
    int Page,
    int Size)
{
    public GetChildrenDepartmentsQuery ToQuery(Guid parentId) =>
        new (parentId, Page, Size);
}