using FluentValidation;
using SharedService.Core.Validation;
using SharedService.SharedKernel.Errors;

namespace DirectoryService.Application.Test;

public class TestValidator:
    AbstractValidator<TestCommand>
{
    public TestValidator()
    {
        RuleFor(c => c.TestId)
            .NotNull()
            .NotEmpty()
            .WithError(Errors.General.ValueIsRequired());

        RuleFor(c => c.Comment)
            .NotNull()
            .NotEmpty()
            .WithError(Errors.General.ValueIsRequired());
    }
}