using CSharpFunctionalExtensions;
using Dapper;
using DirectoryService.Application.Shared.DTOs;
using FluentValidation;
using SharedService.Core.Abstractions;
using SharedService.Core.Database.Intefraces;
using SharedService.Core.Validation;
using SharedService.SharedKernel.Errors;

namespace DirectoryService.Application.Queries.Departments.GetTopPositions;

public class GetTopPositionsHandler(
    IValidator<GetTopPositionsQuery> validator,
    IDBConnectionFactory connectionFactory)
    : IQueryHandlerWithResult<GetTopPositionsResponse, GetTopPositionsQuery>
{
    public async Task<Result<GetTopPositionsResponse, ErrorList>> Handle(
        GetTopPositionsQuery query, CancellationToken cancellationToken = default)
    {
        var validationResult = await validator.ValidateAsync(
            query, cancellationToken);
        if (validationResult.IsValid == false)
            return validationResult.ToList();

        var parameters = new DynamicParameters();
        parameters.Add("@limit", query.Limit);

        using var connection = connectionFactory.Create();

        var result = await connection.QueryAsync<DepartmentWithPositionsDto>(new CommandDefinition(
            commandText: """
                         WITH dp AS (
                             SELECT 
                                 department_id,
                                 COUNT(*) as positions_count
                             FROM directory_service.department_positions
                             GROUP BY department_id
                         )
                         SELECT d.id,
                                d.name,
                                d.path,
                                d.depth,
                                d.created_at,
                                d.updated_at,
                                dp.positions_count 
                         FROM directory_service.departments d
                         JOIN dp ON d.id = dp.department_id
                         ORDER BY dp.positions_count DESC
                         LIMIT @limit
                         """,
            parameters: parameters,
            cancellationToken: cancellationToken));

        return new GetTopPositionsResponse { DepartmentWithPositions = result.ToList() };
    }
}