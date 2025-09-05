using FluentValidation;
using SharedService.Core.Validation;
using SharedService.SharedKernel.Errors;

namespace DirectoryService.Application.Queries.Departments.GetTopPositions;

public class GetTopPositionsValidator : AbstractValidator<GetTopPositionsQuery>
{
    public GetTopPositionsValidator()
    {
        RuleFor(q => q.Limit)
            .Must(d => d > 0 && d < 1000)
            .WithError(Errors.General.ValueIsInvalid(nameof(GetTopPositionsQuery.Limit)));
    }
}