using CSharpFunctionalExtensions;
using DirectoryService.Domain.Models;
using Microsoft.EntityFrameworkCore;
using SharedService.SharedKernel.BaseClasses;
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

    public Task<Result<IEnumerable<Department>>> GetChildrenByPathAsync(
        LTree path,
        CancellationToken cancellationToken = default);

    Task<Result<int, Error>> UpdateChildrenPathAsync(
        LTree oldPath, short oldPathDepth,
        LTree newPath, short newPathDepth,
        CancellationToken cancellationToken = default);

    public Task<Result<IEnumerable<Department>>> GetDepartmentsForLocationAsync(
        Id<Location> id,
        CancellationToken cancellationToken = default);

    public Task<UnitResult<Error>> AreDepartmentsValidAsync(
        IEnumerable<Id<Department>> departmentIds,
        CancellationToken cancellationToken = default);
}