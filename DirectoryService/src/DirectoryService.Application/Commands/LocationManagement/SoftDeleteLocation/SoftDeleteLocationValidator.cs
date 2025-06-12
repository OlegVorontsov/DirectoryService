using FluentValidation;
using SharedService.Core.Validation;
using SharedService.SharedKernel.Errors;

namespace DirectoryService.Application.Commands.LocationManagement.SoftDeleteLocation;

public class SoftDeleteLocationValidator : AbstractValidator<SoftDeleteLocationCommand>
{
    public SoftDeleteLocationValidator()
    {
        RuleFor(c => c.Id)
            .NotEmpty()
            .Must(d => d != Guid.Empty)
            .WithError(Errors.General.ValueIsRequired("Id"));
    }
}