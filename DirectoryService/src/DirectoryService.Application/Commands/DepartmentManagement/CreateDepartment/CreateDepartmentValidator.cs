using DirectoryService.Domain.ValueObjects.DepartmentValueObjects;
using FluentValidation;
using SharedService.Core.Validation;
using SharedService.SharedKernel.Errors;

namespace DirectoryService.Application.Commands.DepartmentManagement.CreateDepartment;

public class CreateDepartmentValidator : AbstractValidator<CreateDepartmentCommand>
{
    public CreateDepartmentValidator()
    {
        RuleFor(c => c.Name)
            .MustBeValueObject(DepartmentName.Create);

        RuleFor(c => c.ParentId)
            .Must(d => d != Guid.Empty)
            .WithError(Errors.General.ValueIsInvalid("ParentId"));

        RuleFor(c => c.LocationIds)
            .NotEmpty()
            .WithError(Errors.General.ValueIsRequired("LocationIds"));
    }
}