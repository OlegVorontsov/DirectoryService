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
    IPositionRepository positionRepository,
    IDepartmentRepository departmentRepository)
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

        // validate departments
        var departmentIds = command.DepartmentIds.Select(Id<Department>.Create);
        var areLocationsValidResult = await departmentRepository.AreDepartmentsValidAsync(
            departmentIds,
            cancellationToken);
        if (areLocationsValidResult.IsFailure)
            return areLocationsValidResult.Error.ToErrors();

        var entity = Position.Create(
                id: Id<Position>.GenerateNew(),
                name: name,
                description: PositionDescription.Create(command.Description).Value,
                createdAt: DateTime.UtcNow).Value;

        entity.UpdateDepartments(departmentIds);

        var result = await positionRepository.CreateAsync(
            entity, cancellationToken);

        if (result.IsFailure)
            return Errors.General.Failure(result.Error).ToErrors();

        logger.LogInformation(
            "Position created with id {0} name {1}",
            result.Value.Id.Value,
            result.Value.Name.Value);

        return PositionDTO.FromDomainEntity(result.Value);
    }
}