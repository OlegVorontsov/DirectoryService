using CSharpFunctionalExtensions;
using DirectoryService.Application.Interfaces.Repositories;
using DirectoryService.Domain.Models;
using DirectoryService.Domain.Shared.BaseClasses;
using DirectoryService.Domain.ValueObjects.DepartmentValueObjects;
using Microsoft.EntityFrameworkCore;
using SharedService.SharedKernel.Errors;

namespace DirectoryService.Infrastructure.DataBase.Repositories;

public class DepartmentRepository(ApplicationDBContext context) : IDepartmentRepository
{
    public async Task<Result<Department>> CreateAsync(
        Department entity, CancellationToken cancellationToken = default)
    {
        context.Departments.Add(entity);
        await context.SaveChangesAsync(cancellationToken);

        return entity;
    }

    public async Task<Result<Department, Error>> UpdateAsync(
        Department entity,
        CancellationToken cancellationToken = default,
        IEnumerable<DepartmentLocation>? oldDepartmentLocations = null)
    {
        try
        {
            // sync DepartmentLocations
            if (oldDepartmentLocations is not null)
            {
                foreach (var item in oldDepartmentLocations)
                {
                    var found = entity.DepartmentLocations
                        .FirstOrDefault(d => d.LocationId == item.LocationId);
                    if (found is null)
                        context.DepartmentLocations.Remove(item);
                }

                // this part was used to add new DepartmentLocations
                // after refactoring it is not needed anymore
                //foreach (var newItem in entity.DepartmentLocations)
                //{
                //    var found = oldDepartmentLocations
                //        .FirstOrDefault(d => d.LocationId == newItem.LocationId);
                //    if (found is null)
                //        _context.DepartmentLocations.Add(newItem);
                //}
            }

            // Not needed because entity is tracked by EF Core,
            // and it wil auto update it when SaveChanges is called
            // var entry = _context.Departments.Update(entity);
            await context.SaveChangesAsync(cancellationToken);
            return entity;
        }
        catch (DbUpdateConcurrencyException)
        {
            return Errors.General.Failure($"ConcurrentUpdateFailed {entity.Id.Value}");
        }
    }

    public async Task<Result<Department, Error>> GetByIdAsync(
        Id<Department> id, CancellationToken cancellationToken = default)
    {
        var entity = await context.Departments
            .Include(d => d.Parent)
            .Include(d => d.DepartmentLocations)
            .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
        if (entity is null)
            return Errors.General.NotFound(id.Value);

        return entity;
    }

    public async Task<UnitResult<Error>> IsPathUniqueAsync(
        LTree path, CancellationToken cancellationToken = default)
    {
        var entity = await context.Departments
            .Where(d => d.IsActive)
            .FirstOrDefaultAsync(d => d.Path == path, cancellationToken);
        return entity is not null ?
            Errors.General.AlreadyExists(path) :
            UnitResult.Success<Error>();
    }

    public async Task<Result<IEnumerable<Department>>> GetFlatTreeAsync(
        LTree path, CancellationToken cancellationToken = default)
    {
        var result = await context.Departments
            .Where(d => d.IsActive)
            .Where(d => d.Path.IsAncestorOf(path)) // also returns itself
            .ToListAsync(cancellationToken);

        return result;
    }

    public async Task<UnitResult<Error>> UpdateChildrenPathAsync(
        LTree oldPath, LTree newPath, CancellationToken cancellationToken = default)
    {
        try
        {
            var oldDepth = Department.CalculateDepth(oldPath);
            var newDepth = Department.CalculateDepth(newPath);

            // Intention:
            // UPDATE item
            // SET path = NEW.path || subpath(path, nlevel(OLD.path))
            // WHERE path<@ OLD.path;
            await context.Departments
                .Where(d => d.IsActive)
                .Where(d => d.Path.IsDescendantOf(oldPath))
                .Where(d => d.Path != newPath)
                .ExecuteUpdateAsync(
                    propCall => propCall
                        .SetProperty(
                            d => d.Path,
                            d => (LTree)(newPath + "." + d.Path.Subpath(oldDepth + 1)))
                        .SetProperty(
                            d => d.Depth,
                            d => d.Path.NLevel - 1 - oldDepth + newDepth),  // d.Path.NLevel - 1 won't work, because it will not be updated yet
                    cancellationToken);

            return UnitResult.Success<Error>();
        }
        catch (DbUpdateConcurrencyException)
        {
            return Errors.General.Failure($"ConcurrentUpdateFailed {oldPath}");
        }
    }
}