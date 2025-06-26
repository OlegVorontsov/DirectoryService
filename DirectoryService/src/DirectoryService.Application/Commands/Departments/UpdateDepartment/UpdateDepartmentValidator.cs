using DirectoryService.Domain.ValueObjects.Departments;
using FluentValidation;
using SharedService.Core.Validation;
using SharedService.SharedKernel.Errors;

namespace DirectoryService.Application.Commands.Departments.UpdateDepartment;

public class UpdateDepartmentValidator : AbstractValidator<UpdateDepartmentCommand>
{
    public UpdateDepartmentValidator()
    {
        RuleFor(c => c.Id)
            .NotEmpty()
            .Must(d => d != Guid.Empty)
            .WithError(Errors.General.ValueIsRequired("Id"));

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