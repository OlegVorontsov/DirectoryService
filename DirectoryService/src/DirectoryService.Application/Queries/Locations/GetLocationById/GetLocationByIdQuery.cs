using SharedService.Core.Abstractions;

namespace DirectoryService.Application.Queries.Locations.GetLocationById;

public record GetLocationByIdQuery(
    Guid Id) : IQuery;