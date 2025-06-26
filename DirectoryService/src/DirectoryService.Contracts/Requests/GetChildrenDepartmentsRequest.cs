using DirectoryService.Application.Queries.Departments.GetChildrenDepartments;

namespace DirectoryService.Contracts.Requests;

public record GetChildrenDepartmentsRequest(
    int Page,
    int Size)
{
    public GetChildrenDepartmentsQuery ToQuery(Guid parentId) =>
        new (parentId, Page, Size);
}