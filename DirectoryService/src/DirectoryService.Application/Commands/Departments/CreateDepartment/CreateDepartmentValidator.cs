using DirectoryService.Domain.ValueObjects.Departments;
using FluentValidation;
using SharedService.Core.Validation;
using SharedService.SharedKernel.Errors;

namespace DirectoryService.Application.Commands.Departments.CreateDepartment;

public class CreateDepartmentValidator : AbstractValidator<CreateDepartmentCommand>
{
    public CreateDepartmentValidator()
    {
        RuleFor(c => c.Name)
            .MustBeValueObject(DepartmentName.Create);

        RuleFor(c => c.ParentId)
            .Must(d => d != Guid.Empty)
            .WithError(Errors.General.ValueIsInvalid("ParentId"));

        RuleForEach(c => c.LocationIds)
            .NotNull()
            .NotEmpty()
            .Must(d => d != Guid.Empty)
            .WithError(Errors.General.ValueIsRequired("LocationIds"));
    }
}