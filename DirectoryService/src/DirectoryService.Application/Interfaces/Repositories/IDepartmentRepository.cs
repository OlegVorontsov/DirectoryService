using CSharpFunctionalExtensions;
using DirectoryService.Domain.Models;
using DirectoryService.Domain.Shared.BaseClasses;
using Microsoft.EntityFrameworkCore;
using SharedService.SharedKernel.Errors;

namespace DirectoryService.Application.Interfaces.Repositories;

public interface IDepartmentRepository
{
    Task<Result<Department>> CreateAsync(
        Department entity,
        CancellationToken cancellationToken = default);

    Task<Result<Department>> UpdateAsync(
        Department entity,
        CancellationToken cancellationToken = default);

    Task<Result<Department, Error>> GetByIdAsync(
        Id<Department> id,
        CancellationToken cancellationToken = default);

    Task<UnitResult<Error>> IsPathUniqueAsync(
        LTree path,
        CancellationToken cancellationToken = default);

    Task<Result<IEnumerable<Department>>> GetFlatTreeAsync(
        LTree path, CancellationToken cancellationToken = default);

    Task<UnitResult<Error>> UpdateChildrenPathAsync(
        LTree oldPath, short oldPathDepth,
        LTree newPath, short newPathDepth,
        CancellationToken cancellationToken = default);
}