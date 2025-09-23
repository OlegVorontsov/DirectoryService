using FluentValidation;
using SharedService.Core.Validation;
using SharedService.SharedKernel.Errors;

namespace DirectoryService.Application.Queries.Departments.GetDepartmentById;

public class GetDepartmentByIdValidator : AbstractValidator<GetDepartmentByIdQuery>
{
    public GetDepartmentByIdValidator()
    {
        RuleFor(c => c.Id)
            .NotEmpty()
            .Must(d => d != Guid.Empty)
            .WithError(Errors.General.ValueIsRequired("Id"));
    }
}