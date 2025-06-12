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

namespace DirectoryService.Application.Commands.LocationManagement.SoftDeleteLocation;

public class SoftDeleteLocationHandler(
    IValidator<SoftDeleteLocationCommand> validator,
    ILogger<SoftDeleteLocationHandler> logger,
    ILocationRepository locationRepository,
    IDepartmentRepository departmentRepository,
    IUnitOfWork unitOfWork)
    : ICommandHandler<Guid, SoftDeleteLocationCommand>
{
    public async Task<Result<Guid, ErrorList>> Handle(
        SoftDeleteLocationCommand command, CancellationToken cancellationToken = default)
    {
        var validationResult = await validator.ValidateAsync(
            command, cancellationToken);
        if (validationResult.IsValid == false)
            return validationResult.ToList();

        // check if entity exists
        var entityResult = await locationRepository.GetByIdAsync(
            id: Id<Location>.Create(command.Id),
            cancellationToken: cancellationToken);
        if (entityResult.IsFailure)
            return entityResult.Error.ToErrors();

        var entity = entityResult.Value;

        var departmentsResult = await departmentRepository.GetDepartmentsForLocationAsync(
            entity.Id, cancellationToken);
        if (departmentsResult.IsFailure)
            return Errors.General.Failure(departmentsResult.Error).ToErrors();
        if (departmentsResult.Value.Any())
            return Errors.General.DeleteConflict(entity.Id.Value, typeof(Location).ToString()).ToErrors();

        entity.Deactivate();
        var entityUpdateResult = await locationRepository.UpdateAsync(
            entity, cancellationToken);
        if (entityUpdateResult.IsFailure)
            return Errors.General.Failure(entityUpdateResult.Error).ToErrors();

        logger.LogInformation("Location with id {0} was deactivated", entity.Id);

        return entity.Id.Value;
    }
}