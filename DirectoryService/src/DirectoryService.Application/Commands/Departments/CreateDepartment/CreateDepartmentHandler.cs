using CSharpFunctionalExtensions;
using DirectoryService.Application.Interfaces.DataBase;
using DirectoryService.Application.Interfaces.Repositories;
using DirectoryService.Application.Shared.DTOs;
using DirectoryService.Domain.Models;
using DirectoryService.Domain.Shared.BaseClasses;
using DirectoryService.Domain.ValueObjects.Departments;
using FluentValidation;
using Microsoft.Extensions.Logging;
using SharedService.Core.Abstractions;
using SharedService.Core.Validation;
using SharedService.SharedKernel.Errors;

namespace DirectoryService.Application.Commands.Departments.CreateDepartment;

public class CreateDepartmentHandler(
    IValidator<CreateDepartmentCommand> validator,
    ILogger<CreateDepartmentHandler> logger,
    IDepartmentRepository departmentRepository,
    ILocationRepository locationRepository,
    IUnitOfWork unitOfWork)
    : ICommandHandler<DepartmentDTO, CreateDepartmentCommand>
{
    public async Task<Result<DepartmentDTO, ErrorList>> Handle(
        CreateDepartmentCommand command,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(
            command, cancellationToken);
        if (validationResult.IsValid == false)
            return validationResult.ToList();

        // validate locations
        var locationIds = command.LocationIds.Select(Id<Location>.Create);
        var areLocationsValidResult = await locationRepository.AreLocationsValidAsync(
            locationIds, cancellationToken);
        if (areLocationsValidResult.IsFailure)
            return areLocationsValidResult.Error.ToErrors();

        Id<Department>? parentId = null;
        Department? parent = null;
        if (command.ParentId is not null)
        {
            // validate parent
            parentId = Id<Department>.Create(command.ParentId.Value);
            var parentResult = await departmentRepository.GetByIdAsync(
                parentId, cancellationToken);
            if (parentResult.IsFailure) return parentResult.Error.ToErrors();

            parent = parentResult.Value;
        }

        var name = DepartmentName.Create(command.Name).Value;

        var pathResult = Department.CreatePath(name, parent?.Path);
        if (pathResult.IsFailure) return pathResult.Error.ToErrors();

        var isPathUnique = await departmentRepository.IsPathUniqueAsync(
            pathResult.Value, cancellationToken);
        if (isPathUnique.IsFailure) return isPathUnique.Error.ToErrors();

        var departmentResult = Department.Create(
            id: Id<Department>.GenerateNew(),
            name: name,
            parent: parent,
            createdAt: DateTime.UtcNow);

        if (departmentResult.IsFailure) return departmentResult.Error.ToErrors();
        var entity = departmentResult.Value;

        var transaction = await unitOfWork.BeginTransactionAsync(cancellationToken);

        entity.UpdateLocations(locationIds);

        var createResult = await departmentRepository.CreateAsync(entity, cancellationToken);
        if (createResult.IsFailure)
        {
            transaction.Rollback();
            return Errors.General.Failure(createResult.Error).ToErrors();
        }

        if (parent is not null)
        {
            parent.IncreaseChildrenCount();
            var updateParentResult = await departmentRepository.UpdateAsync(parent, cancellationToken);
            if (updateParentResult.IsFailure)
            {
                transaction.Rollback();
                return Errors.General.Failure("updateParent").ToErrors();
            }
        }

        transaction.Commit();

        logger.LogInformation(
            "Department created with id {0} name {1}",
            entity.Name.Value,
            entity.Id.Value);

        return DepartmentDTO.FromDomainEntity(entity);
    }
}