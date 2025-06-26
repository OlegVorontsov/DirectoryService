using SharedService.Core.Abstractions;

namespace DirectoryService.Application.Queries.LocationManagement.GetLocations;

public record GetLocationsQuery(
    int Page,
    int Size,
    string? Search = null) : IQuery;