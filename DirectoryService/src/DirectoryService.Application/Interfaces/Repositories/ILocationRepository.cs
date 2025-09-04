using CSharpFunctionalExtensions;
using DirectoryService.Domain.Models;
using DirectoryService.Domain.ValueObjects.Locations;
using SharedService.SharedKernel.BaseClasses;
using SharedService.SharedKernel.Errors;

namespace DirectoryService.Application.Interfaces.Repositories;

public interface ILocationRepository
{
    Task<Result<Location, Error>> GetByIdAsync(
        Id<Location> id,
        CancellationToken cancellationToken = default);

    Task<Result<Location, Error>> GetByNameAsync(
        LocationName name,
        CancellationToken cancellationToken = default);

    public Task<Result<(int TotalCount, IEnumerable<Location> Values)>> GetAsync(
        Func<IQueryable<Location>, IQueryable<Location>> filterQuery,
        int Page,
        int PageSize,
        CancellationToken cancellationToken = default);

    Task<Result<IEnumerable<Location>>> GetLocationsForDepartmentAsync(
        Id<Department> id,
        CancellationToken cancellationToken = default);

    Task<Result<Location>> CreateAsync(
        Location entity,
        CancellationToken cancellationToken = default);

    Task<Result<Location>> UpdateAsync(
        Location entity,
        CancellationToken cancellationToken = default);

    Task<UnitResult<Error>> IsNameUniqueAsync(
        LocationName name,
        CancellationToken cancellationToken = default);

    public Task<UnitResult<Error>> IsAddressUniqueAsync(
        LocationAddress address,
        CancellationToken cancellationToken = default);

    Task<bool> AreLocationsValidAsync(
        IEnumerable<Id<Location>> locationIds,
        CancellationToken cancellationToken = default);
}