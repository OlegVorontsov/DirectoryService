using CSharpFunctionalExtensions;
using DirectoryService.Domain.Models;
using DirectoryService.Domain.Shared.BaseClasses;
using DirectoryService.Domain.ValueObjects.Positions;
using SharedService.SharedKernel.Errors;

namespace DirectoryService.Application.Interfaces.Repositories;

public interface IPositionRepository
{
    Task<Result<Position, Error>> GetByIdAsync(
        Id<Position> id,
        CancellationToken cancellationToken = default);

    Task<Result<Position, Error>> GetByNameAsync(
        PositionName name,
        CancellationToken cancellationToken = default);

    Task<Result<Position>> CreateAsync(
        Position entity,
        CancellationToken cancellationToken = default);

    Task<Result<Position>> UpdateAsync(
        Position entity,
        CancellationToken cancellationToken = default,
        IEnumerable<DepartmentPosition>? oldDepartmentPositions = null);

    Task<UnitResult<Error>> IsNameUniqueAsync(
        PositionName name,
        CancellationToken cancellationToken = default);
}