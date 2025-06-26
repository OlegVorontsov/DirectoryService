using CSharpFunctionalExtensions;
using Dapper;
using DirectoryService.Application.Shared.DTOs;
using DirectoryService.Application.Shared.Interfaces;
using FluentValidation;
using Microsoft.Extensions.Logging;
using SharedService.Core.Abstractions;
using SharedService.Core.Validation;
using SharedService.SharedKernel.Errors;

namespace DirectoryService.Application.Queries.LocationManagement.GetLocationById;

public class GetLocationByIdHandler(
    IValidator<GetLocationByIdQuery> validator,
    ILogger<GetLocationByIdHandler> logger,
    IDBConnectionFactory connectionFactory)
    : IQueryHandlerWithResult<LocationDTO, GetLocationByIdQuery>
{
    public async Task<Result<LocationDTO, ErrorList>> Handle(
        GetLocationByIdQuery query, CancellationToken cancellationToken = default)
    {
        var validationResult = await validator.ValidateAsync(
            query, cancellationToken);
        if (validationResult.IsValid == false)
            return validationResult.ToList();

        var parameters = new DynamicParameters();
        parameters.Add("@id", query.Id);

        using var connection = connectionFactory.Create();

        var result = await connection.QueryFirstOrDefaultAsync<LocationDTO>(new CommandDefinition(
            commandText: """
                         SELECT id, name, address, time_zone
                         FROM directory_service.locations
                         WHERE id = @id and is_active = true
                         """,
            parameters: parameters,
            cancellationToken: cancellationToken));
        if (result is null)
            return Errors.General.NotFound(query.Id).ToErrors();

        logger.LogInformation("Location with id {id} queried", query.Id);

        return result;
    }
}