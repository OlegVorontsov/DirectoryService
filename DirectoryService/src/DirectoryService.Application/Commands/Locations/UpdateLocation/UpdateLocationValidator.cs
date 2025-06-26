using DirectoryService.Domain.ValueObjects.Locations;
using FluentValidation;
using SharedService.Core.Validation;
using SharedService.SharedKernel.Errors;

namespace DirectoryService.Application.Commands.Locations.UpdateLocation;

public class UpdateLocationValidator : AbstractValidator<UpdateLocationCommand>
{
    public UpdateLocationValidator()
    {
        RuleFor(c => c.Id)
            .NotEmpty()
            .Must(d => d != Guid.Empty)
            .WithError(Errors.General.ValueIsRequired("Id"));

        RuleFor(c => c.Name)
            .MustBeValueObject(LocationName.Create);

        RuleFor(c => c.Address)
            .MustBeValueObject(a => LocationAddress.Create(
                a.City, a.Street, a.HouseNumber));

        RuleFor(c => c.TimeZone)
            .MustBeValueObject(IANATimeZone.Create);
    }
}