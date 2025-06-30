using CSharpFunctionalExtensions;
using DirectoryService.Application.Interfaces.Repositories;
using DirectoryService.Domain.Models;
using DirectoryService.Domain.Shared.BaseClasses;
using DirectoryService.Domain.ValueObjects.Positions;
using DirectoryService.Infrastructure.DataBase.Write;
using Microsoft.EntityFrameworkCore;
using SharedService.SharedKernel.Errors;

namespace DirectoryService.Infrastructure.DataBase.Repositories;

public class PositionRepository(ApplicationWriteDBContext context) : IPositionRepository
{
    public async Task<Result<Position, Error>> GetByIdAsync(
        Id<Position> id,
        CancellationToken cancellationToken = default)
    {
        var entity = await context.Positions
            .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
        if (entity is null)
            return Errors.General.NotFound(id.Value);

        return entity;
    }

    public async Task<Result<Position, Error>> GetByNameAsync(
        PositionName name,
        CancellationToken cancellationToken = default)
    {
        var entity = await context.Positions
            .FirstOrDefaultAsync(d => d.Name == name, cancellationToken);
        if (entity is null)
            return Errors.General.NotFound();

        return entity;
    }

    public async Task<Result<Position>> CreateAsync(
        Position entity, CancellationToken cancellationToken = default)
    {
        context.Positions.Add(entity);
        await context.SaveChangesAsync(cancellationToken);

        return entity;
    }

    public async Task<Result<Position>> UpdateAsync(
        Position entity,
        CancellationToken cancellationToken = default,
        IEnumerable<DepartmentPosition>? oldDepartmentPositions = null)
    {
        // sync DepartmentPositions
        if (oldDepartmentPositions is not null)
        {
            foreach (var item in oldDepartmentPositions)
            {
                var found = entity.DepartmentPositions
                    .FirstOrDefault(d => d.PositionId == item.PositionId);
                if (found is null)
                    context.DepartmentPositions.Remove(item);
            }
        }

        await context.SaveChangesAsync(cancellationToken);

        return entity;
    }

    public async Task<UnitResult<Error>> IsNameUniqueAsync(
        PositionName name, CancellationToken cancellationToken = default)
    {
        var existingPosition = await context.Positions
            .FirstOrDefaultAsync(d => d.Name == name, cancellationToken);

        if (existingPosition is not null)
            return Errors.General.AlreadyExists(name.Value);

        return UnitResult.Success<Error>();
    }
}