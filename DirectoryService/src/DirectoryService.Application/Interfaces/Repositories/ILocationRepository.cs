using CSharpFunctionalExtensions;
using DirectoryService.Domain.Models;
using DirectoryService.Domain.Shared.BaseClasses;
using DirectoryService.Domain.ValueObjects.LocationValueObjects;
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

    Task<UnitResult<Error>> AreLocationsValidAsync(
        IEnumerable<Id<Location>> locationIds,
        CancellationToken cancellationToken = default);
}