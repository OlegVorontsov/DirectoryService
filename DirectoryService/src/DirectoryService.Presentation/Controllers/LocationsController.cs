using DirectoryService.Application.Commands.LocationManagement.CreateLocation;
using DirectoryService.Application.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;
using SharedService.Framework.EndpointResults;

namespace DirectoryService.Presentation.Controllers;

public class LocationsController : ApplicationController
{
    [HttpPost]
    public async Task<EndpointResult<LocationDTO>> Create(
        [FromServices] CreateLocationHandler handler,
        [FromBody] CreateLocationCommand command,
        CancellationToken cancellationToken = default) =>
        await handler.Handle(command, cancellationToken);
}