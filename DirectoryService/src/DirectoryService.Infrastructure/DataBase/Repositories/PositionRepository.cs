using CSharpFunctionalExtensions;
using DirectoryService.Application.Interfaces.Repositories;
using DirectoryService.Domain.Models;
using DirectoryService.Domain.ValueObjects.PositionValueObjects;
using Microsoft.EntityFrameworkCore;
using SharedService.SharedKernel.Errors;

namespace DirectoryService.Infrastructure.DataBase.Repositories;

public class PositionRepository(ApplicationDBContext context) : IPositionRepository
{
    public async Task<Result<Position>> CreateAsync(
        Position entity, CancellationToken cancellationToken = default)
    {
        context.Positions.Add(entity);
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