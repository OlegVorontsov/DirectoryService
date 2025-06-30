using CSharpFunctionalExtensions;
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

namespace DirectoryService.Application.Queries.Locations.GetLocations;

public class GetLocationsHandler(
    IValidator<GetLocationsQuery> validator,
    ILogger<GetLocationsHandler> logger,
    IDBConnectionFactory connectionFactory)
    : IQueryHandlerWithResult<FilteredListDTO<LocationDTO>, GetLocationsQuery>
{
    public async Task<Result<FilteredListDTO<LocationDTO>, ErrorList>> Handle(
        GetLocationsQuery query, CancellationToken cancellationToken = default)
    {
        var validationResult = await validator.ValidateAsync(
            query, cancellationToken);
        if (validationResult.IsValid == false)
            return validationResult.ToList();

        using var connection = connectionFactory.Create();

        var totalCountBuilder = new CustomSQLBuilder(
            """
            SELECT count(id)
            FROM directory_service.locations
            WHERE is_active = true
            """);
        if (string.IsNullOrEmpty(query.Search) == false)
        {
            totalCountBuilder.Append(" AND");
            totalCountBuilder.AddTextSearchCondition("name", query.Search);
        }

        var totalCount = await connection.ExecuteScalarAsync<int>(
            totalCountBuilder,
            logger,
            cancellationToken);

        var selectBuilder = new CustomSQLBuilder(
            """
            SELECT id, name, address, time_zone
            FROM directory_service.locations
            WHERE is_active = true
            """);
        if (string.IsNullOrEmpty(query.Search) == false)
        {
            selectBuilder.Append(" AND");
            selectBuilder.AddTextSearchCondition("name", query.Search);
        }

        selectBuilder
            .ApplySorting(new Dictionary<string, bool>
            {
                { "name", true },
            })
            .ApplyPagination(query.Page, query.Size);

        var selectResult = await connection.QueryAsync<LocationDTO>(
            selectBuilder,
            logger,
            cancellationToken);

        return new FilteredListDTO<LocationDTO>(
            query.Page,
            query.Size,
            selectResult,
            totalCount);
    }
}