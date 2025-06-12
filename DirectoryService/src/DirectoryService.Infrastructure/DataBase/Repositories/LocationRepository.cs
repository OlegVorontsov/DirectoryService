using CSharpFunctionalExtensions;
using DirectoryService.Application.Interfaces.Repositories;
using DirectoryService.Domain.Models;
using DirectoryService.Domain.Shared.BaseClasses;
using DirectoryService.Domain.ValueObjects.LocationValueObjects;
using Microsoft.EntityFrameworkCore;
using SharedService.SharedKernel.Errors;

namespace DirectoryService.Infrastructure.DataBase.Repositories;

public class LocationRepository(
    ApplicationDBContext context) : ILocationRepository
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