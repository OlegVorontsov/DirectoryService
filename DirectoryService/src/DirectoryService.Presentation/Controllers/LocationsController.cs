using DirectoryService.Application.Commands.LocationManagement.CreateLocation;
using DirectoryService.Application.Commands.LocationManagement.SoftDeleteLocation;
using DirectoryService.Application.Commands.LocationManagement.UpdateLocation;
using DirectoryService.Application.Shared.DTOs;
using DirectoryService.Presentation.Requests;
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

    [HttpPut("{id:guid}")]
    public async Task<EndpointResult<LocationDTO>> Update(
        [FromServices] UpdateLocationHandler handler,
        [FromBody] UpdateLocationRequest request,
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default) =>
        await handler.Handle(request.ToCommand(id), cancellationToken);

    [HttpDelete("{id:guid}")]
    public async Task<EndpointResult<Guid>> Update(
        [FromServices] SoftDeleteLocationHandler handler,
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default) =>
        await handler.Handle(new SoftDeleteLocationCommand(id), cancellationToken);
}