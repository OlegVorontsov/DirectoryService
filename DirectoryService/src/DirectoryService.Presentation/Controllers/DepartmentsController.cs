using DirectoryService.Application.Commands.Departments.CreateDepartment;
using DirectoryService.Application.Commands.Departments.MoveDepartment;
using DirectoryService.Application.Commands.Departments.SoftDeleteDepartment;
using DirectoryService.Application.Commands.Departments.UpdateDepartment;
using DirectoryService.Application.Queries.Departments.GetChildrenDepartments;
using DirectoryService.Application.Queries.Departments.GetRootDepartments;
using DirectoryService.Application.Shared.DTOs;
using DirectoryService.Contracts.Requests;
using Microsoft.AspNetCore.Mvc;
using SharedService.Core.DTOs;
using SharedService.Framework;
using SharedService.Framework.EndpointResults;

namespace DirectoryService.Presentation.Controllers;

public class DepartmentsController : ApplicationController
{
    [HttpPost]
    public async Task<EndpointResult<DepartmentDTO>> Create(
        [FromServices] CreateDepartmentHandler handler,
        [FromBody] CreateDepartmentCommand command,
        CancellationToken cancellationToken = default) =>
        await handler.Handle(command, cancellationToken);

    [HttpPut("{id:guid}")]
    public async Task<EndpointResult<DepartmentDTO>> Update(
        [FromServices] UpdateDepartmentHandler handler,
        [FromBody] UpdateDepartmentRequest request,
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default) =>
        await handler.Handle(request.ToCommand(id), cancellationToken);

    [HttpPut("move/{id:guid}")]
    public async Task<EndpointResult<DepartmentDTO>> Move(
        [FromServices] MoveDepartmentHandler handler,
        [FromBody] MoveDepartmentRequest request,
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default) =>
        await handler.Handle(request.ToCommand(id), cancellationToken);

    [HttpDelete("{id:guid}")]
    public async Task<EndpointResult<Guid>> Delete(
        [FromServices] SoftDeleteDepartmentHandler handler,
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default) =>
        await handler.Handle(new SoftDeleteDepartmentCommand(id), cancellationToken);

    [HttpGet("roots")]
    public async Task<EndpointResult<FilteredListDTO<DepartmentTreeDTO>>> GetRoots(
        [FromServices] GetRootDepartmentsHandler handler,
        [FromQuery] GetRootDepartmentsQuery query,
        CancellationToken cancellationToken = default) =>
        await handler.Handle(query, cancellationToken);

    [HttpGet("{id:guid}/children")]
    public async Task<EndpointResult<FilteredListDTO<DepartmentDTO>>> GetChildren(
        [FromServices] GetChildrenDepartmentsHandler handler,
        [FromRoute] Guid id,
        [FromQuery] GetChildrenDepartmentsRequest request,
        CancellationToken cancellationToken = default) =>
        await handler.Handle(request.ToQuery(id), cancellationToken);
}