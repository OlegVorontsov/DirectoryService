using CSharpFunctionalExtensions;
using DirectoryService.Application.Interfaces.Caching;
using DirectoryService.Application.Shared.DTOs;
using DirectoryService.Domain;
using FluentValidation;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using SharedService.Core.Abstractions;
using SharedService.Core.Database.Builders;
using SharedService.Core.Database.Extensions;
using SharedService.Core.Database.Intefraces;
using SharedService.Core.DTOs;
using SharedService.Core.Validation;
using SharedService.SharedKernel.Errors;

namespace DirectoryService.Application.Queries.Departments.GetChildrenDepartments;

public class GetChildrenDepartmentsHandler(
    IValidator<GetChildrenDepartmentsQuery> validator,
    ILogger<GetChildrenDepartmentsHandler> logger,
    IDBConnectionFactory connectionFactory,
    ICacheService cache)
    : IQueryHandlerWithResult<FilteredListDTO<DepartmentDTO>, GetChildrenDepartmentsQuery>
{
    private readonly DistributedCacheEntryOptions _cacheOptions = new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(Constants.CacheConstants.DEFAULT_EXPIRATION_MINUTES),
    };

    public async Task<Result<FilteredListDTO<DepartmentDTO>, ErrorList>> Handle(
        GetChildrenDepartmentsQuery query, CancellationToken cancellationToken = default)
    {
        var validationResult = await validator.ValidateAsync(
            query, cancellationToken);
        if (validationResult.IsValid == false)
            return validationResult.ToList();

        var key = Constants.CacheConstants.DEPARTMENTS_PREFIX + query.ParentId;
        var result = await cache.GetOrSetAsync(
            key,
            _cacheOptions,
            async () => await GetDepartmentDtos(query, cancellationToken),
            cancellationToken);

        if (result is null)
            return Errors.General.NotFound().ToErrors();

        return new FilteredListDTO<DepartmentDTO>(
            query.Page,
            query.Size,
            result.Departments,
            result.TotalCount);
    }

    private async Task<GetChildrenDepartmentsDto> GetDepartmentDtos(
        GetChildrenDepartmentsQuery query, CancellationToken cancellationToken)
    {
        using var connection = connectionFactory.Create();

        var totalCountBuilder = new CustomSQLBuilder(
            """
            SELECT count(id)
            FROM directory_service.departments
            WHERE is_active = true AND parent_id = @parent_id
            """);
        totalCountBuilder.Parameters.Add("@parent_id", query.ParentId);

        var totalCount = await connection.ExecuteScalarAsync<int>(
            totalCountBuilder,
            logger,
            cancellationToken);

        var selectBuilder = new CustomSQLBuilder(
            """
            SELECT id, name, parent_id, path, depth, children_count, is_active, created_at, updated_at
            FROM directory_service.departments
            WHERE is_active = true AND parent_id = @parent_id
            """);
        selectBuilder.Parameters.Add("@parent_id", query.ParentId);

        selectBuilder.ApplySorting(new Dictionary<string, bool> {
            { "name", true },
        }).ApplyPagination(query.Page, query.Size);

        var selectResult = await connection.QueryAsync<DepartmentDTO>(
            selectBuilder,
            logger,
            cancellationToken);

        return new GetChildrenDepartmentsDto(totalCount, selectResult);
    }
}