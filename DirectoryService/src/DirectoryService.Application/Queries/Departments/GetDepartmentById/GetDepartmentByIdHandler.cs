using CSharpFunctionalExtensions;
using Dapper;
using DirectoryService.Application.Shared.DTOs;
using FluentValidation;
using Microsoft.Extensions.Logging;
using SharedService.Core.Abstractions;
using SharedService.Core.Database.Intefraces;
using SharedService.Core.Validation;
using SharedService.SharedKernel.Errors;

namespace DirectoryService.Application.Queries.Departments.GetDepartmentById;

public class GetDepartmentByIdHandler(
    IValidator<GetDepartmentByIdQuery> validator,
    ILogger<GetDepartmentByIdHandler> logger,
    IDBConnectionFactory connectionFactory)
    : IQueryHandlerWithResult<DepartmentDTO, GetDepartmentByIdQuery>
{
    public async Task<Result<DepartmentDTO, ErrorList>> Handle(
        GetDepartmentByIdQuery query, CancellationToken cancellationToken = default)
    {
        var validationResult = await validator.ValidateAsync(
            query, cancellationToken);
        if (validationResult.IsValid == false)
            return validationResult.ToList();

        var parameters = new DynamicParameters();
        parameters.Add("@id", query.Id);

        using var connection = connectionFactory.Create();

        var result = await connection.QueryFirstOrDefaultAsync<DepartmentDTO>(new CommandDefinition(
            commandText: """
                         SELECT id, name, parent_id, path, depth, children_count, is_active, created_at, updated_at
                         FROM directory_service.departments
                         WHERE id = @id and is_active = true
                         """,
            parameters: parameters,
            cancellationToken: cancellationToken));
        if (result is null)
            return Errors.General.NotFound(query.Id).ToErrors();

        logger.LogInformation("Department with id {id} queried", query.Id);

        return result;
    }
}