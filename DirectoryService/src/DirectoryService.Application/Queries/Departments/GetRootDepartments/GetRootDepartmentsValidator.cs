using FluentValidation;
using SharedService.Core.Validation;
using SharedService.SharedKernel.Errors;

namespace DirectoryService.Application.Queries.Departments.GetRootDepartments;

public class GetRootDepartmentsValidator : AbstractValidator<GetRootDepartmentsQuery>
{
    public GetRootDepartmentsValidator()
    {
        RuleFor(q => q.Page)
            .Must(d => d > 0)
            .WithError(Errors.General.ValueIsInvalid(nameof(GetRootDepartmentsQuery.Page)));

        RuleFor(q => q.Size)
            .Must(d => d > 0 && d < 1000)
            .WithError(Errors.General.ValueIsInvalid(nameof(GetRootDepartmentsQuery.Size)));

        RuleFor(q => q.Prefetch)
            .Must(d => d > 0 && d < 3)
            .WithError(Errors.General.ValueIsInvalid(nameof(GetRootDepartmentsQuery.Prefetch)));
    }
}