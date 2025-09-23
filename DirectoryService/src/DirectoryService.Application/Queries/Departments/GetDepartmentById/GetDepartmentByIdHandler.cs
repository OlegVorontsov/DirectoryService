using CSharpFunctionalExtensions;
using Dapper;
using DirectoryService.Application.Interfaces.Caching;
using DirectoryService.Application.Shared.DTOs;
using DirectoryService.Domain;
using FluentValidation;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using SharedService.Core.Abstractions;
using SharedService.Core.Database.Intefraces;
using SharedService.Core.Validation;
using SharedService.SharedKernel.Errors;

namespace DirectoryService.Application.Queries.Departments.GetDepartmentById;

public class GetDepartmentByIdHandler(
    IValidator<GetDepartmentByIdQuery> validator,
    ILogger<GetDepartmentByIdHandler> logger,
    IDBConnectionFactory connectionFactory,
    ICacheService cache)
    : IQueryHandlerWithResult<DepartmentDTO, GetDepartmentByIdQuery>
{
    private readonly DistributedCacheEntryOptions _cacheOptions = new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(Constants.CacheConstants.DEFAULT_EXPIRATION_MINUTES),
    };

    public async Task<Result<DepartmentDTO, ErrorList>> Handle(
        GetDepartmentByIdQuery query, CancellationToken cancellationToken = default)
    {
        var validationResult = await validator.ValidateAsync(
            query, cancellationToken);
        if (validationResult.IsValid == false)
            return validationResult.ToList();

        var key = Constants.CacheConstants.DEPARTMENTS_PREFIX + query.Id;
        var departmentDto = await cache.GetOrSetAsync(
            key,
            _cacheOptions,
            async () => await GetDepartmentDtoById(query, cancellationToken),
            cancellationToken);

        if (departmentDto is null)
            return Errors.General.NotFound(query.Id).ToErrors();

        logger.LogInformation("Department with id {id} queried", query.Id);

        return departmentDto;
    }

    private async Task<DepartmentDTO?> GetDepartmentDtoById(
        GetDepartmentByIdQuery query, CancellationToken cancellationToken)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@id", query.Id);

        using var connection = connectionFactory.Create();

        return await connection.QueryFirstOrDefaultAsync<DepartmentDTO>(new CommandDefinition(
            commandText: """
                         SELECT id, name, parent_id, path, depth, children_count, is_active, created_at, updated_at
                         FROM directory_service.departments
                         WHERE id = @id and is_active = true
                         """,
            parameters: parameters,
            cancellationToken: cancellationToken));
    }
}