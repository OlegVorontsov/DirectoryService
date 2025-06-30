using FluentValidation;
using SharedService.Core.Validation;
using SharedService.SharedKernel.Errors;

namespace DirectoryService.Application.Queries.Locations.GetLocationById;

public class GetLocationByIdValidator : AbstractValidator<GetLocationByIdQuery>
{
    public GetLocationByIdValidator()
    {
        RuleFor(c => c.Id)
            .NotEmpty()
            .Must(d => d != Guid.Empty)
            .WithError(Errors.General.ValueIsRequired("Id"));
    }
}