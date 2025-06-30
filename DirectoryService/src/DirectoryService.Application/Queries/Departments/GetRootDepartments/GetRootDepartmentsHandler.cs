using CSharpFunctionalExtensions;
using Dapper;
using DirectoryService.Application.Shared.DTOs;
using FluentValidation;
using Microsoft.Extensions.Logging;
using SharedService.Core.Abstractions;
using SharedService.Core.Database.Builders;
using SharedService.Core.Database.Extensions;
using SharedService.Core.Database.Intefraces;
using SharedService.Core.DTOs;
using SharedService.Core.Validation;
using SharedService.SharedKernel.Errors;

namespace DirectoryService.Application.Queries.Departments.GetRootDepartments;

public class GetRootDepartmentsHandler(
    IValidator<GetRootDepartmentsQuery> validator,
    ILogger<GetRootDepartmentsHandler> logger,
    IDBConnectionFactory connectionFactory)
    : IQueryHandlerWithResult<FilteredListDTO<DepartmentTreeDTO>, GetRootDepartmentsQuery>
{
    public async Task<Result<FilteredListDTO<DepartmentTreeDTO>, ErrorList>> Handle(
        GetRootDepartmentsQuery query, CancellationToken cancellationToken = default)
    {
        var validationResult = await validator.ValidateAsync(
            query, cancellationToken);
        if (validationResult.IsValid == false)
            return validationResult.ToList();

        using var connection = connectionFactory.Create();

        var totalCountBuilder = new CustomSQLBuilder(
            """
            SELECT count(id)
            FROM directory_service.departments
            WHERE is_active = true AND depth = 0
            """);

        var totalCount = await connection.ExecuteScalarAsync<int>(
            totalCountBuilder,
            logger,
            cancellationToken);

        var multipleSelectBuilder = new CustomSQLBuilder(
            """
            SELECT id, name, parent_id, path, depth, children_count
            FROM directory_service.departments
            WHERE is_active = true AND depth = 0
            ORDER BY name
            LIMIT @limit OFFSET @offset;
            SELECT *
            FROM (SELECT
                id, name, parent_id, path, depth, children_count,
                ROW_NUMBER () OVER (
                    PARTITION BY parent_id
                    ORDER BY name)
                    AS r_number
                FROM directory_service.departments
                WHERE is_active = true AND depth = 1)
            WHERE r_number <= @prefetch;
            """);
        multipleSelectBuilder.Parameters.Add("@offset", (query.Page - 1) * query.Size);
        multipleSelectBuilder.Parameters.Add("@limit", query.Size);
        multipleSelectBuilder.Parameters.Add("@prefetch", query.Prefetch);

        var result = await connection.QueryMultipleAsync(
            multipleSelectBuilder,
            logger,
            cancellationToken);

        var roots = result.Read<DepartmentTreeDTO>().AsList();
        var children = result.Read<DepartmentTreeDTO>();

        // in memory mapping with dictionary is faster than json aggregation
        // (according to https://medium.com/@nelsonciofi/the-best-way-to-store-and-retrieve-complex-objects-with-dapper-5eff32e6b29e)
        var lookupMap = new Dictionary<Guid, int>();

        for (int i = 0; i < roots.Count; i++)
            lookupMap.Add(roots[i].Id, i);

        foreach (var child in children)
        {
            if (lookupMap.TryGetValue(child.ParentId!.Value, out int index))
                roots[index].Children.Add(child);
        }

        // roots' children now filled
        return new FilteredListDTO<DepartmentTreeDTO>(
            Page: query.Page,
            Size: query.Size,
            Total: totalCount,
            Data: roots);
    }
}