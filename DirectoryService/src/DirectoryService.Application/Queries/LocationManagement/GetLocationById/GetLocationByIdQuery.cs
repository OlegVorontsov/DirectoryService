using SharedService.Core.Abstractions;

namespace DirectoryService.Application.Queries.LocationManagement.GetLocationById;

public record GetLocationByIdQuery(
    Guid Id) : IQuery;