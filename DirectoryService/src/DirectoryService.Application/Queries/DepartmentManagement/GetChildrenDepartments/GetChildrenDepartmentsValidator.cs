using FluentValidation;
using SharedService.Core.Validation;
using SharedService.SharedKernel.Errors;

namespace DirectoryService.Application.Queries.DepartmentManagement.GetChildrenDepartments;

public class GetChildrenDepartmentsValidator : AbstractValidator<GetChildrenDepartmentsQuery>
{
    public GetChildrenDepartmentsValidator()
    {
        RuleFor(q => q.ParentId)
            .NotEmpty()
            .Must(d => d != Guid.Empty)
            .WithError(Errors.General.ValueIsRequired("ParentId"));

        RuleFor(q => q.Page)
            .Must(d => d > 0)
            .WithError(Errors.General.ValueIsInvalid(nameof(GetChildrenDepartmentsQuery.Page)));

        RuleFor(q => q.Size)
            .Must(d => d > 0 && d < 1000)
            .WithError(Errors.General.ValueIsInvalid(nameof(GetChildrenDepartmentsQuery.Size)));
    }
}