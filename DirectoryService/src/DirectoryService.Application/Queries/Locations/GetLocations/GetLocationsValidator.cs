using FluentValidation;
using SharedService.Core.Validation;
using SharedService.SharedKernel.Errors;

namespace DirectoryService.Application.Queries.Locations.GetLocations;

public class GetLocationsValidator : AbstractValidator<GetLocationsQuery>
{
    public GetLocationsValidator()
    {
        RuleFor(q => q.Page)
            .Must(d => d > 0)
            .WithError(Errors.General.ValueIsInvalid(nameof(GetLocationsQuery.Page)));

        RuleFor(q => q.Size)
            .Must(d => d > 0 && d < 1000)
            .WithError(Errors.General.ValueIsInvalid(nameof(GetLocationsQuery.Size)));
    }
}