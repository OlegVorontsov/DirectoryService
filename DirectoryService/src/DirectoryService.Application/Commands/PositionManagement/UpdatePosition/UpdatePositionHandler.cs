using CSharpFunctionalExtensions;
using DirectoryService.Application.Interfaces.Repositories;
using DirectoryService.Application.Shared.DTOs;
using DirectoryService.Domain.Models;
using DirectoryService.Domain.Shared.BaseClasses;
using DirectoryService.Domain.ValueObjects.PositionValueObjects;
using FluentValidation;
using Microsoft.Extensions.Logging;
using SharedService.Core.Abstractions;
using SharedService.Core.Validation;
using SharedService.SharedKernel.Errors;

namespace DirectoryService.Application.Commands.PositionManagement.UpdatePosition;

public class UpdatePositionHandler(
    IValidator<UpdatePositionCommand> validator,
    ILogger<UpdatePositionHandler> logger,
    IPositionRepository positionRepository)
    : ICommandHandler<PositionDTO, UpdatePositionCommand>
{
    public async Task<Result<PositionDTO, ErrorList>> Handle(
        UpdatePositionCommand command, CancellationToken cancellationToken = default)
    {
        var validationResult = await validator.ValidateAsync(
            command, cancellationToken);
        if (validationResult.IsValid == false)
            return validationResult.ToList();

        // check if position exists
        var id = Id<Position>.Create(command.Id);
        var entityResult = await positionRepository.GetByIdAsync(
            id, cancellationToken);
        if (entityResult.IsFailure)
            return entityResult.Error.ToErrors();

        var entity = entityResult.Value;

        // check if new name is unique
        var name = PositionName.Create(command.Name).Value;
        if (entity.Name != name)
        {
            var isNameUnique = await positionRepository.IsNameUniqueAsync(name, cancellationToken);
            if (isNameUnique.IsFailure)
                return isNameUnique.Error.ToErrors();
        }

        entity.Update(
            name: name,
            description: PositionDescription.Create(command.Description).Value);

        var updateResult = await positionRepository.UpdateAsync(entity, cancellationToken);
        if (updateResult.IsFailure)
            return Errors.General.Failure("Failed to update position").ToErrors();

        logger.LogInformation(
            "Position with id {0} name {1} updated",
            updateResult.Value.Id.Value,
            updateResult.Value.Name.Value);

        return PositionDTO.FromDomainEntity(entity);
    }
}