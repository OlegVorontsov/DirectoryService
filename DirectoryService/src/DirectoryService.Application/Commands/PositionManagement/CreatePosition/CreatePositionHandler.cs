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

namespace DirectoryService.Application.Commands.PositionManagement.CreatePosition;

public class CreatePositionHandler(
    IValidator<CreatePositionCommand> validator,
    ILogger<CreatePositionHandler> logger,
    IPositionRepository positionRepository)
    : ICommandHandler<PositionDTO, CreatePositionCommand>
{
    public async Task<Result<PositionDTO, ErrorList>> Handle(
        CreatePositionCommand command,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await validator.ValidateAsync(
            command, cancellationToken);
        if (validationResult.IsValid == false)
            return validationResult.ToList();

        var name = PositionName.Create(command.Name).Value!;
        var isNameUnique = await positionRepository.IsNameUniqueAsync(name, cancellationToken);
        if (isNameUnique.IsFailure)
            return isNameUnique.Error.ToErrors();

        var createResult = await positionRepository.CreateAsync(
            entity: Position.Create(
                id: Id<Position>.GenerateNew(),
                name: name,
                description: PositionDescription.Create(command.Description).Value,
                createdAt: DateTime.UtcNow).Value!,
            cancellationToken: cancellationToken);

        if (createResult.IsFailure)
            return Errors.General.Failure(createResult.Error).ToErrors();

        logger.LogInformation(
            "Position created with id {0} name {1}",
            createResult.Value.Name.Value,
            createResult.Value.Id.Value);

        return PositionDTO.FromDomainEntity(createResult.Value);
    }
}