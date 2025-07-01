using DirectoryService.Application.Commands.Positions.CreatePosition;
using DirectoryService.Application.Commands.Positions.SoftDeletePosition;
using DirectoryService.Application.Commands.Positions.UpdatePosition;
using DirectoryService.Application.Shared.DTOs;
using DirectoryService.Contracts.Requests;
using Microsoft.AspNetCore.Mvc;
using SharedService.Framework;
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