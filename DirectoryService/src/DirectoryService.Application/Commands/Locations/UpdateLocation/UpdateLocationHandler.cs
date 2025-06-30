using CSharpFunctionalExtensions;
using DirectoryService.Application.Interfaces.Repositories;
using DirectoryService.Application.Shared.DTOs;
using DirectoryService.Domain.Models;
using DirectoryService.Domain.ValueObjects.Locations;
using FluentValidation;
using Microsoft.Extensions.Logging;
using SharedService.Core.Abstractions;
using SharedService.Core.Validation;
using SharedService.SharedKernel.BaseClasses;
using SharedService.SharedKernel.Errors;

namespace DirectoryService.Application.Commands.Locations.UpdateLocation;

public class UpdateLocationHandler(
    IValidator<UpdateLocationCommand> validator,
    ILogger<UpdateLocationHandler> logger,
    ILocationRepository locationRepository)
    : ICommandHandler<LocationDTO, UpdateLocationCommand>
{
    public async Task<Result<LocationDTO, ErrorList>> Handle(
        UpdateLocationCommand command,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await validator.ValidateAsync(
            command, cancellationToken);
        if (validationResult.IsValid == false)
            return validationResult.ToList();

        // check if location exists
        var id = Id<Location>.Create(command.Id);
        var entityResult = await locationRepository.GetByIdAsync(
            id, cancellationToken);
        if (entityResult.IsFailure)
            return entityResult.Error.ToErrors();

        var entity = entityResult.Value;

        // check if new name is unique
        var name = LocationName.Create(command.Name).Value;
        if (entity.Name != name)
        {
            var isNameUnique = await locationRepository.IsNameUniqueAsync(name, cancellationToken);
            if (isNameUnique.IsFailure)
                return isNameUnique.Error.ToErrors();
        }

        entity.Update(
            name: name,
            address: LocationAddress.Create(
                city: command.Address.City,
                street: command.Address.Street,
                houseNumber: command.Address.HouseNumber).Value,
            timeZone: IANATimeZone.Create(command.TimeZone).Value);

        var updateResult = await locationRepository.UpdateAsync(entity, cancellationToken);
        if (updateResult.IsFailure)
            return Errors.General.Failure("Failed to update location").ToErrors();

        logger.LogInformation(
            "Location with id {0} name {1} updated",
            updateResult.Value.Id.Value,
            updateResult.Value.Name.Value);

        return LocationDTO.FromDomainEntity(entity);
    }
}