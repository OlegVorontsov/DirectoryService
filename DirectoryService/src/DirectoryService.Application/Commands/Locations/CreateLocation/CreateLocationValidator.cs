using DirectoryService.Domain.ValueObjects.Locations;
using FluentValidation;
using SharedService.Core.Validation;

namespace DirectoryService.Application.Commands.Locations.CreateLocation;

public class CreateLocationValidator : AbstractValidator<CreateLocationCommand>
{
    public CreateLocationValidator()
    {
        RuleFor(c => c.Name)
            .MustBeValueObject(LocationName.Create);

        RuleFor(c => c.Address)
            .MustBeValueObject(a => LocationAddress.Create(
                a.City, a.Street, a.HouseNumber));

        RuleFor(c => c.TimeZone)
            .MustBeValueObject(IANATimeZone.Create);
    }
}