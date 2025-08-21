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

namespace DirectoryService.Application.Commands.Locations.CreateLocation;

public class CreateLocationHandler(
    IValidator<CreateLocationCommand> validator,
    ILogger<CreateLocationHandler> logger,
    ILocationRepository locationRepository)
    : ICommandHandler<LocationDTO, CreateLocationCommand>
{
    public async Task<Result<LocationDTO, ErrorList>> Handle(
        CreateLocationCommand command,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await validator.ValidateAsync(
            command, cancellationToken);
        if (validationResult.IsValid == false)
            return validationResult.ToList();

        var name = LocationName.Create(command.Name).Value!;
        var isNameUnique = await locationRepository.IsNameUniqueAsync(name, cancellationToken);
        if (isNameUnique.IsFailure)
            return isNameUnique.Error.ToErrors();

        var address = LocationAddress.Create(
            command.Address.City,
            command.Address.Street,
            command.Address.HouseNumber).Value!;
        var isAddressUnique = await locationRepository.IsAddressUniqueAsync(address, cancellationToken);
        if (isAddressUnique.IsFailure)
            return isAddressUnique.Error.ToErrors();

        var timeZone = IANATimeZone.Create(command.TimeZone).Value!;

        var createResult = await locationRepository.CreateAsync(
            entity: Location.Create(
                id: Id<Location>.GenerateNew(),
                name: name,
                address: address,
                timeZone: timeZone).Value!,
            cancellationToken: cancellationToken);

        if (createResult.IsFailure)
            return Errors.General.Failure(createResult.Error).ToErrors();

        logger.LogInformation(
            "Location created with id {0} name {1}",
            createResult.Value.Name.Value,
            createResult.Value.Id.Value);

        return LocationDTO.FromDomainEntity(createResult.Value);
    }
}