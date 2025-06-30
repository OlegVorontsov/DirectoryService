using FluentValidation;
using SharedService.Core.Validation;
using SharedService.SharedKernel.Errors;

namespace DirectoryService.Application.Commands.Positions.SoftDeletePosition;

public class SoftDeletePositionValidator : AbstractValidator<SoftDeletePositionCommand>
{
    public SoftDeletePositionValidator()
    {
        RuleFor(c => c.Id)
            .NotEmpty()
            .Must(d => d != Guid.Empty)
            .WithError(Errors.General.ValueIsRequired("Id"));
    }
}