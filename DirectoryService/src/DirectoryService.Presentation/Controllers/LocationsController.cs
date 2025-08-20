using DirectoryService.Application.Commands.Locations.CreateLocation;
using DirectoryService.Application.Commands.Locations.SoftDeleteLocation;
using DirectoryService.Application.Commands.Locations.UpdateLocation;
using DirectoryService.Application.Queries.Locations.GetLocationById;
using DirectoryService.Application.Queries.Locations.GetLocations;
using DirectoryService.Application.Shared.DTOs;
using DirectoryService.Contracts.Requests;
using Microsoft.AspNetCore.Mvc;
using SharedService.Core.DTOs;
using SharedService.Framework;
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
    public async Task<EndpointResult<Guid>> Delete(
        [FromServices] SoftDeleteLocationHandler handler,
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default) =>
        await handler.Handle(new SoftDeleteLocationCommand(id), cancellationToken);

    [HttpGet]
    public async Task<EndpointResult<FilteredListDTO<LocationDTO>>> GetAll(
        [FromServices] GetLocationsHandler handler,
        [FromQuery] GetLocationsQuery query,
        CancellationToken cancellationToken = default) =>
        await handler.Handle(query, cancellationToken);

    [HttpGet("{id:guid}")]
    public async Task<EndpointResult<LocationDTO>> Get(
        [FromServices] GetLocationByIdHandler handler,
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default) =>
        await handler.Handle(new GetLocationByIdQuery(id), cancellationToken);
}