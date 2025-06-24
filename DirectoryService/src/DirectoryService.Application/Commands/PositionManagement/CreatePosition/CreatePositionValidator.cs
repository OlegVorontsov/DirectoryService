using DirectoryService.Domain.ValueObjects.PositionValueObjects;
using FluentValidation;
using SharedService.Core.Validation;
using SharedService.SharedKernel.Errors;

namespace DirectoryService.Application.Commands.PositionManagement.CreatePosition;

public class CreatePositionValidator : AbstractValidator<CreatePositionCommand>
{
    public CreatePositionValidator()
    {
        RuleFor(c => c.Name)
            .MustBeValueObject(PositionName.Create);

        RuleFor(c => c.Description)
            .MustBeValueObject(PositionDescription.Create);

        RuleFor(c => c.DepartmentIds)
            .NotEmpty()
            .WithError(Errors.General.ValueIsRequired("DepartmentIds"));
    }
}