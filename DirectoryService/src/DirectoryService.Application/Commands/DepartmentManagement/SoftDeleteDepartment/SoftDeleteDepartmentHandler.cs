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

namespace DirectoryService.Application.Commands.DepartmentManagement.SoftDeleteDepartment;

public class SoftDeleteDepartmentHandler(
    IValidator<SoftDeleteDepartmentCommand> validator,
    ILogger<SoftDeleteDepartmentHandler> logger,
    IDepartmentRepository departmentRepository,
    ILocationRepository locationRepository,
    IUnitOfWork unitOfWork)
    : ICommandHandler<Guid, SoftDeleteDepartmentCommand>
{
    public async Task<Result<Guid, ErrorList>> Handle(
        SoftDeleteDepartmentCommand command, CancellationToken cancellationToken = default)
    {
        var validationResult = await validator.ValidateAsync(
            command, cancellationToken);
        if (validationResult.IsValid == false)
            return validationResult.ToList();

        // check if entity exists
        var entityResult = await departmentRepository.GetByIdAsync(
            id: Id<Department>.Create(command.Id),
            cancellationToken: cancellationToken);
        if (entityResult.IsFailure)
            return entityResult.Error.ToErrors();

        var entity = entityResult.Value;

        // check if entity has children
        var childrenResult = await departmentRepository.GetChildrenByPathAsync(
            entity.Path,
            cancellationToken);
        if (childrenResult.IsFailure) return Errors.General.Failure(childrenResult.Error).ToErrors();
        if (childrenResult.Value.Any())
            return Errors.General.DeleteConflict(entity.Id.Value, typeof(Department).ToString()).ToErrors();

        var transaction = await unitOfWork.BeginTransactionAsync(cancellationToken);

        if (entity.Parent is not null)
        {
            entity.Parent.DecreaseChildrenCount();
            var parentUpdateResult = await departmentRepository.UpdateAsync(
                entity.Parent,
                cancellationToken);
            if (parentUpdateResult.IsFailure)
                return Errors.General.Failure(parentUpdateResult.Error).ToErrors();
        }

        entity.Deactivate();
        var entityUpdateResult = await departmentRepository.UpdateAsync(
            entity, cancellationToken);
        if (entityUpdateResult.IsFailure)
        {
            transaction.Rollback();
            return Errors.General.Failure(entityUpdateResult.Error).ToErrors();
        }

        transaction.Commit();

        logger.LogInformation("Department with id {id} was deactivated", entity.Id);
        if (entity.Parent is not null)
            logger.LogInformation("Parent with id {id} was updated", entity.Parent.Id);

        return entity.Id.Value;
    }
}