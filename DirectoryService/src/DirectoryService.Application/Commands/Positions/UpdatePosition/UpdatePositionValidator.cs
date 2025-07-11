﻿using DirectoryService.Domain.ValueObjects.Positions;
using FluentValidation;
using SharedService.Core.Validation;
using SharedService.SharedKernel.Errors;

namespace DirectoryService.Application.Commands.Positions.UpdatePosition;

public class UpdatePositionValidator : AbstractValidator<UpdatePositionCommand>
{
    public UpdatePositionValidator()
    {
        RuleFor(c => c.Id)
            .NotEmpty()
            .Must(d => d != Guid.Empty)
            .WithError(Errors.General.ValueIsRequired("Id"));

        RuleFor(c => c.Name)
            .MustBeValueObject(PositionName.Create);

        RuleFor(c => c.Description)
            .MustBeValueObject(PositionDescription.Create);

        RuleFor(c => c.DepartmentIds)
            .NotEmpty()
            .WithError(Errors.General.ValueIsRequired("DepartmentIds"));
    }
}