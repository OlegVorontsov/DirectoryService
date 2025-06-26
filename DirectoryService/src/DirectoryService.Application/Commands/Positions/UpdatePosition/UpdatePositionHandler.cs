using CSharpFunctionalExtensions;
using DirectoryService.Application.Interfaces.Repositories;
using DirectoryService.Application.Shared.DTOs;
using DirectoryService.Domain.Models;
using DirectoryService.Domain.Shared.BaseClasses;
using DirectoryService.Domain.ValueObjects.Positions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using SharedService.Core.Abstractions;
using SharedService.Core.Validation;
using SharedService.SharedKernel.Errors;

namespace DirectoryService.Application.Commands.Positions.UpdatePosition;

public class UpdatePositionHandler(
    IValidator<UpdatePositionCommand> validator,
    ILogger<UpdatePositionHandler> logger,
    IPositionRepository positionRepository,
    IDepartmentRepository departmentRepository)
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
        var isNameChanged = entity.Name != name;
        if (isNameChanged)
        {
            var isNameUnique = await positionRepository.IsNameUniqueAsync(name, cancellationToken);
            if (isNameUnique.IsFailure)
                return isNameUnique.Error.ToErrors();
        }

        var description = PositionDescription.Create(command.Description).Value;
        var isDescriptionChanged = entity.Description != description;

        // validate positions
        var areDepartmentsChanged = entity.AreDepartmentsChanged(command.DepartmentIds);
        if (areDepartmentsChanged)
        {
            var departmentIds = command.DepartmentIds.Select(Id<Department>.Create);
            var areDepartmentsValidResult = await departmentRepository.AreDepartmentsValidAsync(
                departmentIds,
                cancellationToken);
            if (areDepartmentsValidResult.IsFailure)
                return areDepartmentsValidResult.Error.ToErrors();
        }

        if ((isNameChanged || isDescriptionChanged || areDepartmentsChanged) == false)
        {
            logger.LogInformation(
                "Position with id {id} was unchanged, because request didn't have any changes",
                entity.Id);
            return PositionDTO.FromDomainEntity(entity);
        }

        if (isNameChanged)
        {
            entity.Update(
                name: name,
                description: entity.Description);
        }

        if (isDescriptionChanged)
        {
            entity.Update(
                name: entity.Name,
                description: description);
        }

        var oldDepartments = entity.DepartmentPositions;
        if (areDepartmentsChanged)
            entity.UpdateDepartments(command.DepartmentIds.Select(Id<Department>.Create));

        var updateResult = await positionRepository.UpdateAsync(
            entity,
            cancellationToken,
            oldDepartments);
        if (updateResult.IsFailure)
            return Errors.General.Failure("Failed to update position").ToErrors();

        logger.LogInformation(
            "Position with id {0} name {1} updated",
            updateResult.Value.Id.Value,
            updateResult.Value.Name.Value);

        return PositionDTO.FromDomainEntity(entity);
    }
}