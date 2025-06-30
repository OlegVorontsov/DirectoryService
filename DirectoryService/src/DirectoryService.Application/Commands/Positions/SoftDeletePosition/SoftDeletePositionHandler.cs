using CSharpFunctionalExtensions;
using DirectoryService.Application.Interfaces.DataBase;
using DirectoryService.Application.Interfaces.Repositories;
using DirectoryService.Domain.Models;
using DirectoryService.Domain.Shared.BaseClasses;
using FluentValidation;
using Microsoft.Extensions.Logging;
using SharedService.Core.Abstractions;
using SharedService.Core.Validation;
using SharedService.SharedKernel.Errors;

namespace DirectoryService.Application.Commands.Positions.SoftDeletePosition;

public class SoftDeletePositionHandler(
    IValidator<SoftDeletePositionCommand> validator,
    ILogger<SoftDeletePositionHandler> logger,
    IPositionRepository positionRepository,
    IUnitOfWork unitOfWork)
    : ICommandHandler<Guid, SoftDeletePositionCommand>
{
    public async Task<Result<Guid, ErrorList>> Handle(
        SoftDeletePositionCommand command, CancellationToken cancellationToken = default)
    {
        var validationResult = await validator.ValidateAsync(
            command, cancellationToken);
        if (validationResult.IsValid == false)
            return validationResult.ToList();

        // check if entity exists
        var entityResult = await positionRepository.GetByIdAsync(
            id: Id<Position>.Create(command.Id),
            cancellationToken: cancellationToken);
        if (entityResult.IsFailure)
            return entityResult.Error.ToErrors();

        var entity = entityResult.Value;

        entity.Deactivate();

        var updateResult = await positionRepository.UpdateAsync(entity, cancellationToken);
        if (updateResult.IsFailure)
            return Errors.General.Failure(updateResult.Error).ToErrors();

        logger.LogInformation("Position with id {id} was deactivated", entity.Id);

        return entity.Id.Value;
    }
}