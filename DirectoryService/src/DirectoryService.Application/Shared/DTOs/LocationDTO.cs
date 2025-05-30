using DirectoryService.Domain.Models;

namespace DirectoryService.Application.Shared.DTOs;

public record LocationDTO(
    Guid Id,
    string Name,
    AddressDTO Address,
    string TimeZone)
{
    public static LocationDTO FromDomainEntity(Location entity)
        => new(
            Id: entity.Id.Value,
            Name: entity.Name.Value,
            Address: new AddressDTO(
                entity.Address.City,
                entity.Address.Street,
                entity.Address.HouseNumber),
            TimeZone: entity.TimeZone.Value);
}