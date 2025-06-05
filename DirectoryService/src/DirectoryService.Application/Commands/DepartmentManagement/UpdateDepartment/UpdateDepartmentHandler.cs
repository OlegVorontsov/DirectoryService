using CSharpFunctionalExtensions;
using DirectoryService.Application.Interfaces.DataBase;
using DirectoryService.Application.Interfaces.Repositories;
using DirectoryService.Application.Shared.DTOs;
using DirectoryService.Domain.Models;
using DirectoryService.Domain.Shared.BaseClasses;
using DirectoryService.Domain.ValueObjects.DepartmentValueObjects;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharedService.Core.Abstractions;
using SharedService.Core.Validation;
using SharedService.SharedKernel.Errors;

namespace DirectoryService.Application.Commands.DepartmentManagement.UpdateDepartment;

public class UpdateDepartmentHandler(
    IValidator<UpdateDepartmentCommand> validator,
    ILogger<UpdateDepartmentHandler> logger,
    IDepartmentRepository departmentRepository,
    ILocationRepository locationRepository,
    IUnitOfWork unitOfWork)
    : ICommandHandler<DepartmentDTO, UpdateDepartmentCommand>
{
    public async Task<Result<DepartmentDTO, ErrorList>> Handle(
        UpdateDepartmentCommand command,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await validator.ValidateAsync(
            command, cancellationToken);
        if (validationResult.IsValid == false)
            return validationResult.ToList();

        // check if entity exists
        var existResult = await departmentRepository.GetByIdAsync(
            id: Id<Department>.Create(command.Id),
            cancellationToken: cancellationToken);
        if (existResult.IsFailure)
            return existResult.Error.ToErrors();

        var entity = existResult.Value;

        var newName = DepartmentName.Create(command.Name).Value;
        bool isNameChanged = entity.Name.Value != newName.Value;

        var updatedParentId = command.ParentId is null ?
                              null :
                              Id<Department>.Create(command.ParentId!.Value);
        bool isParentChanged = entity.ParentId != updatedParentId;

        // Get new parent if not null
        Department? newParent = null;
        if (isParentChanged && updatedParentId is not null)
        {
            var parentResult = await departmentRepository.GetByIdAsync(
                id: updatedParentId,
                cancellationToken: cancellationToken);
            if (parentResult.IsFailure)
                return parentResult.Error.ToErrors();

            newParent = parentResult.Value;

            var isTreeWithoutCyclesResult = await IsTreeWithoutCyclesAsync(
                entity,
                newParent.Path,
                cancellationToken);
            if (isTreeWithoutCyclesResult.IsFailure)
                return isTreeWithoutCyclesResult.Error.ToErrors();
        }

        // validate new Path
        var updatedPathResult = Department.CreatePath(
            isNameChanged ? newName : entity.Name,
            isParentChanged ? newParent?.Path : entity.Parent?.Path);
        if (updatedPathResult.IsFailure)
            return updatedPathResult.Error.ToErrors();

        // validate locations
        var areLocationsChanged = AreLocationChanged(entity, command);
        if (areLocationsChanged)
        {
            var locationIds = command.LocationIds.Select(Id<Location>.Create);
            var areLocationsValidResult = await locationRepository.AreLocationsValidAsync(
                locationIds,
                cancellationToken);
            if (areLocationsValidResult.IsFailure)
                return areLocationsValidResult.Error.ToErrors();
        }

        // ef entity is unchanged return
        if ((isNameChanged || isParentChanged || areLocationsChanged) == false)
            return DepartmentDTO.FromDomainEntity(entity);

        var transaction = await unitOfWork.BeginTransactionAsync(cancellationToken);

        var oldParent = entity.Parent;
        var oldPath = entity.Path;
        var oldLocations = entity.DepartmentLocations;

        // update entity
        if (isNameChanged || isParentChanged)
        {
            entity.Update(
                name: isNameChanged ? newName : entity.Name,
                parentId: isParentChanged ? newParent?.Id : entity.ParentId,
                path: updatedPathResult.Value);
        }

        if (areLocationsChanged)
            entity.UpdateLocations(command.LocationIds.Select(Id<Location>.Create));

        // update old parent if not null
        if (isParentChanged && oldParent is not null)
        {
            oldParent.DecreaseChildrenCount();
            var oldParentUpdateResult = await departmentRepository
                .UpdateAsync(oldParent, cancellationToken);
            if (oldParentUpdateResult.IsFailure)
            {
                transaction.Rollback();
                return oldParentUpdateResult.Error.ToErrors();
            }
        }

        // update new parent if not null
        if (isParentChanged && newParent is not null)
        {
            newParent.IncreaseChildrenCount();
            var newParentUpdateResult = await departmentRepository
                .UpdateAsync(newParent, cancellationToken);
            if (newParentUpdateResult.IsFailure)
            {
                transaction.Rollback();
                return newParentUpdateResult.Error.ToErrors();
            }
        }

        // update entity in database
        if (isNameChanged || isParentChanged || areLocationsChanged)
        {
            var entityUpdateResult = await departmentRepository.UpdateAsync(
                entity,
                cancellationToken,
                oldLocations);
            if (entityUpdateResult.IsFailure)
            {
                transaction.Rollback();
                return entityUpdateResult.Error.ToErrors();
            }
        }

        // update children if parent changed
        if (isNameChanged || isParentChanged)
        {
            var childrenUpdateResult = await departmentRepository.UpdateChildrenPathAsync(
                oldPath,
                entity.Path,
                cancellationToken);
            if (childrenUpdateResult.IsFailure)
            {
                transaction.Rollback();
                return childrenUpdateResult.Error.ToErrors();
            }
        }

        transaction.Commit();

        logger.LogInformation(
            "Department with id {0} name {1} updated",
            entity.Name.Value,
            entity.Id.Value);

        return DepartmentDTO.FromDomainEntity(entity);
    }

    private async Task<UnitResult<Error>> IsTreeWithoutCyclesAsync(
        Department entity,
        LTree parentPath,
        CancellationToken cancellationToken = default)
    {
        // check if tree already contains entity's id
        var parentFlatTreeResult = await departmentRepository.GetFlatTreeAsync(parentPath, cancellationToken);
        if (parentFlatTreeResult.IsFailure)
            return Errors.General.Failure($"parentFlatTreeResult {parentPath}");

        return parentFlatTreeResult.Value
            .FirstOrDefault(d => d.Id == entity.Id) is not null ?
            Errors.General.Failure($"CycleInTree {entity.Id.Value}") :
            UnitResult.Success<Error>();
    }

    private bool AreLocationChanged(
        Department entity,
        UpdateDepartmentCommand command)
    {
        if (entity.DepartmentLocations.Count != command.LocationIds.Count())
            return true;

        for (int i = 0; i < entity.DepartmentLocations.Count; i++)
        {
            if (entity.DepartmentLocations[i].LocationId.Value != command.LocationIds.ElementAt(i))
                return true;
        }

        return false;
    }
}