using DirectoryService.Application.Commands.DepartmentManagement.CreateDepartment;
using DirectoryService.Application.Commands.DepartmentManagement.SoftDeleteDepartment;
using DirectoryService.Application.Commands.DepartmentManagement.UpdateDepartment;
using DirectoryService.Application.Shared.DTOs;
using DirectoryService.Presentation.Requests;
using Microsoft.AspNetCore.Mvc;
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

    [HttpDelete("{id:guid}")]
    public async Task<EndpointResult<Guid>> Delete(
        [FromServices] SoftDeleteDepartmentHandler handler,
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default) =>
        await handler.Handle(new SoftDeleteDepartmentCommand(id), cancellationToken);
}