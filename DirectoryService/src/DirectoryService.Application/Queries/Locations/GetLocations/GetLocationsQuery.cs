using SharedService.Core.Abstractions;

namespace DirectoryService.Application.Queries.Locations.GetLocations;

public record GetLocationsQuery(
    int Page,
    int Size,
    string? Search = null) : IQuery;