using DirectoryService.Application.Commands.PositionManagement.CreatePosition;
using DirectoryService.Application.Commands.PositionManagement.SoftDeletePosition;
using DirectoryService.Application.Commands.PositionManagement.UpdatePosition;
using DirectoryService.Application.Shared.DTOs;
using DirectoryService.Presentation.Requests;
using Microsoft.AspNetCore.Mvc;
using SharedService.Framework.EndpointResults;

namespace DirectoryService.Presentation.Controllers;

public class PositionsController : ApplicationController
{
    [HttpPost]
    public async Task<EndpointResult<PositionDTO>> Create(
        [FromServices] CreatePositionHandler handler,
        [FromBody] CreatePositionCommand command,
        CancellationToken cancellationToken = default) =>
        await handler.Handle(command, cancellationToken);

    [HttpPut("{id:guid}")]
    public async Task<EndpointResult<PositionDTO>> Update(
        [FromServices] UpdatePositionHandler handler,
        [FromBody] UpdatePositionRequest request,
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default) =>
        await handler.Handle(request.ToCommand(id), cancellationToken);

    [HttpDelete("{id:guid}")]
    public async Task<EndpointResult<Guid>> Update(
        [FromServices] SoftDeletePositionHandler handler,
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default) =>
        await handler.Handle(new SoftDeletePositionCommand(id), cancellationToken);
}