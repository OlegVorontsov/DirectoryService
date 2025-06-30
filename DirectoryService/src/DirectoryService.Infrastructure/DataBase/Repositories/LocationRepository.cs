using CSharpFunctionalExtensions;
using DirectoryService.Application.Interfaces.Repositories;
using DirectoryService.Domain.Models;
using DirectoryService.Domain.Shared.BaseClasses;
using DirectoryService.Domain.ValueObjects.Locations;
using DirectoryService.Infrastructure.DataBase.Write;
using Microsoft.EntityFrameworkCore;
using SharedService.SharedKernel.Errors;

namespace DirectoryService.Infrastructure.DataBase.Repositories;

public class LocationRepository(
    ApplicationWriteDBContext context) : ILocationRepository
{
    public async Task<Result<Location, Error>> GetByIdAsync(
        Id<Location> id,
        CancellationToken cancellationToken = default)
    {
        var entity = await context.Locations
            .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
        if (entity is null)
            return Errors.General.NotFound(id.Value);

        return entity;
    }

    public async Task<Result<Location, Error>> GetByNameAsync(
        LocationName name,
        CancellationToken cancellationToken = default)
    {
        var entity = await context.Locations
            .FirstOrDefaultAsync(d => d.Name == name, cancellationToken);
        if (entity is null)
            return Errors.General.NotFound();

        return entity;
    }

    public async Task<Result<(int TotalCount, IEnumerable<Location> Values)>> GetAsync(
        Func<IQueryable<Location>, IQueryable<Location>> filterQuery,
        int Page,
        int PageSize,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Location> set = context.Locations;
        if (filterQuery is not null)
            set = filterQuery(set);

        var totalCount = await set.CountAsync(cancellationToken);

        var result = await set
            .Skip((Page - 1) * PageSize).Take(PageSize)
            .ToListAsync(cancellationToken);

        return (totalCount, result);
    }

    public async Task<Result<IEnumerable<Location>>> GetLocationsForDepartmentAsync(
        Id<Department> id,
        CancellationToken cancellationToken = default)
    {
        var result = await (
            from l in context.Locations
            join dl in context.DepartmentLocations on l.Id equals dl.LocationId
            where dl.DepartmentId == id
            select l).ToListAsync(cancellationToken);

        return result;
    }

    public async Task<Result<Location>> CreateAsync(
        Location entity, CancellationToken cancellationToken = default)
    {
        context.Locations.Add(entity);
        await context.SaveChangesAsync(cancellationToken);

        return entity;
    }

    public async Task<Result<Location>> UpdateAsync(
        Location entity,
        CancellationToken cancellationToken = default)
    {
        await context.SaveChangesAsync(cancellationToken);

        return entity;
    }

    public async Task<UnitResult<Error>> IsNameUniqueAsync(
        LocationName name, CancellationToken cancellationToken = default)
    {
        var existingLocation = await context.Locations.
            FirstOrDefaultAsync(d => d.Name == name, cancellationToken);

        return existingLocation is null ?
            UnitResult.Success<Error>() :
            Errors.General.AlreadyExists(name.Value);
    }

    public async Task<UnitResult<Error>> IsAddressUniqueAsync(
        LocationAddress address,
        CancellationToken cancellationToken = default)
    {
        var existingLocation = await context.Locations
            .FirstOrDefaultAsync(d => d.Address.City == address.City &&
                                      d.Address.Street == address.Street &&
                                      d.Address.HouseNumber == address.HouseNumber, cancellationToken);

        if (existingLocation is not null)
            return Errors.General.AlreadyExists($"{address.City} {address.Street} {address.HouseNumber}");

        return UnitResult.Success<Error>();
    }

    public async Task<UnitResult<Error>> AreLocationsValidAsync(
        IEnumerable<Id<Location>> locationIds, CancellationToken cancellationToken = default)
    {
        var existingIds = await context.Locations
            .Where(l => locationIds.Contains(l.Id))
            .Select(l => l.Id)
            .ToListAsync(cancellationToken);

        foreach (var id in existingIds.Where(id => locationIds.FirstOrDefault(id) is null))
        {
            return Errors.General.NotFound(id.Value);
        }

        return UnitResult.Success<Error>();
    }
}