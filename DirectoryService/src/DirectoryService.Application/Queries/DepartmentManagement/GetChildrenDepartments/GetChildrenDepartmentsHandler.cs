using CSharpFunctionalExtensions;
using Dapper;
using DirectoryService.Application.Shared.Database;
using DirectoryService.Application.Shared.DTOs;
using DirectoryService.Application.Shared.Extensions;
using DirectoryService.Application.Shared.Interfaces;
using FluentValidation;
using Microsoft.Extensions.Logging;
using SharedService.Core.Abstractions;
using SharedService.Core.Validation;
using SharedService.SharedKernel.Errors;

namespace DirectoryService.Application.Queries.DepartmentManagement.GetChildrenDepartments;

public class GetChildrenDepartmentsHandler(
    IValidator<GetChildrenDepartmentsQuery> validator,
    ILogger<GetChildrenDepartmentsHandler> logger,
    IDBConnectionFactory connectionFactory)
    : IQueryHandlerWithResult<FilteredListDTO<DepartmentDTO>, GetChildrenDepartmentsQuery>
{
    public async Task<Result<FilteredListDTO<DepartmentDTO>, ErrorList>> Handle(
        GetChildrenDepartmentsQuery query, CancellationToken cancellationToken = default)
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
            WHERE is_active = true AND parent_id = @parent_id
            """);
        totalCountBuilder.Parameters.Add("@parent_id", query.ParentId);

        var totalCount = await connection.ExecuteScalarAsync<int>(
            totalCountBuilder,
            logger,
            cancellationToken);

        var selectBuilder = new CustomSQLBuilder(
            """
            SELECT id, name, parent_id, path, depth, children_count
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

        return new FilteredListDTO<DepartmentDTO>(
            query.Page,
            query.Size,
            selectResult,
            totalCount);
    }
}