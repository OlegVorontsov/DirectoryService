using CSharpFunctionalExtensions;
using DirectoryService.Application.Interfaces.Repositories;
using DirectoryService.Domain.Models;
using DirectoryService.Infrastructure.DataBase.Write;
using Microsoft.EntityFrameworkCore;
using SharedService.SharedKernel.BaseClasses;
using SharedService.SharedKernel.Errors;

namespace DirectoryService.Infrastructure.DataBase.Repositories;

public class DepartmentRepository(ApplicationWriteDBContext context) : IDepartmentRepository
{
    public async Task<Result<Department>> CreateAsync(
        Department entity, CancellationToken cancellationToken = default)
    {
        context.Departments.Add(entity);
        await context.SaveChangesAsync(cancellationToken);

        return entity;
    }

    public async Task<Result<Department>> UpdateAsync(
        Department entity,
        CancellationToken cancellationToken = default)
    {
        await context.SaveChangesAsync(cancellationToken);
        return entity;
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

    public async Task<Result<IEnumerable<Department>>> GetChildrenByPathAsync(
        LTree path,
        CancellationToken cancellationToken = default)
    {
        var result = await context.Departments
            .Where(d => d.IsActive)
            .Where(d => d.Path.IsDescendantOf(path))
            .Where(d => d.Path != path)
            .ToListAsync(cancellationToken);

        return result;
    }

    public async Task<Result<int, Error>> UpdateChildrenPathAsync(
        LTree oldPath, short oldPathDepth,
        LTree newPath, short newPathDepth,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Intention:
            // UPDATE item
            // SET path = NEW.path || subpath(path, nlevel(OLD.path))
            // WHERE path<@ OLD.path;
            var changedCount = await context.Departments
                .Where(d => d.IsActive)
                .Where(d => d.Path.IsDescendantOf(oldPath))
                .Where(d => d.Path != newPath)
                .ExecuteUpdateAsync(
                    propCall => propCall
                        .SetProperty(
                            d => d.Path,
                            d => (LTree)(newPath + "." + d.Path.Subpath(oldPathDepth + 1)))
                        .SetProperty(
                            d => d.Depth,
                            d => d.Path.NLevel - 1 - oldPathDepth + newPathDepth),  // d.Path.NLevel - 1 won't work, because it will not be updated yet
                    cancellationToken);

            return changedCount;
        }
        catch (DbUpdateConcurrencyException)
        {
            return Errors.General.Failure($"ConcurrentUpdateFailed {oldPath}");
        }
    }

    public async Task<Result<IEnumerable<Department>>> GetDepartmentsForLocationAsync(
        Id<Location> id,
        CancellationToken cancellationToken = default)
    {
        var result = await (
            from d in context.Departments
            join dl in context.DepartmentLocations on d.Id equals dl.DepartmentId
            where dl.LocationId == id
            select d).ToListAsync(cancellationToken);

        return result;
    }

    public async Task<UnitResult<Error>> AreDepartmentsValidAsync(
        IEnumerable<Id<Department>> departmentIds,
        CancellationToken cancellationToken = default)
    {
        var existingIds = await context.Departments
            .Where(d => departmentIds.Contains(d.Id))
            .Select(d => d.Id)
            .ToListAsync(cancellationToken);

        foreach (var id in existingIds)
        {
            if (departmentIds.FirstOrDefault(id) is null)
                return Errors.General.NotFound(id.Value);
        }

        return UnitResult.Success<Error>();
    }
}