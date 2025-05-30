using DirectoryService.Domain.ValueObjects.LocationValueObjects;
using FluentValidation;
using SharedService.Core.Validation;

namespace DirectoryService.Application.Commands.LocationManagement.CreateLocation;

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