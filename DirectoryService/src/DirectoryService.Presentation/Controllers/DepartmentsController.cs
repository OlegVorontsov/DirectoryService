using DirectoryService.Application.Commands.DepartmentManagement.CreateDepartment;
using DirectoryService.Application.Shared.DTOs;
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
}