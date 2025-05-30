using DirectoryService.Domain.ValueObjects.PositionValueObjects;
using FluentValidation;
using SharedService.Core.Validation;

namespace DirectoryService.Application.Commands.PositionManagement.CreatePosition;

public class CreatePositionValidator : AbstractValidator<CreatePositionCommand>
{
    public CreatePositionValidator()
    {
        RuleFor(c => c.Name)
            .MustBeValueObject(PositionName.Create);

        RuleFor(c => c.Description)
            .MustBeValueObject(PositionDescription.Create);
    }
}