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

    public async Task<Result<Department>> UpdateAsync(
        Department entity, CancellationToken cancellationToken = default)
    {
        await context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task<Result<Department, Error>> GetByIdAsync(
        Id<Department> id, CancellationToken cancellationToken = default)
    {
        var entity = await context.Departments
            .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
        if (entity is null)
            return Errors.General.NotFound(id.Value);

        return entity;
    }

    public async Task<UnitResult<Error>> IsPathUniqueAsync(
        DepartmentPath path, CancellationToken cancellationToken = default)
    {
        var entity = await context.Departments
            .FirstOrDefaultAsync(d => d.Path == path, cancellationToken);
        return entity is not null ?
            Errors.General.AlreadyExists(path.Value) :
            UnitResult.Success<Error>();
    }
}