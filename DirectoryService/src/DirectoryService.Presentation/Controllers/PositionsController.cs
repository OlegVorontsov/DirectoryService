using DirectoryService.Application.Commands.PositionManagement.CreatePosition;
using DirectoryService.Application.Shared.DTOs;
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
}