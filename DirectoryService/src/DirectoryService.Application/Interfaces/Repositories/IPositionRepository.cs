using CSharpFunctionalExtensions;
using DirectoryService.Domain.Models;
using DirectoryService.Domain.ValueObjects.PositionValueObjects;
using SharedService.SharedKernel.Errors;

namespace DirectoryService.Application.Interfaces.Repositories;

public interface IPositionRepository
{
    Task<Result<Position>> CreateAsync(
        Position entity,
        CancellationToken cancellationToken = default);

    Task<UnitResult<Error>> IsNameUniqueAsync(
        PositionName name,
        CancellationToken cancellationToken = default);
}