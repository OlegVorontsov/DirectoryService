using DirectoryService.Application.Commands.Locations.UpdateLocation;
using DirectoryService.Application.Shared.DTOs;

namespace DirectoryService.Contracts.Requests;

public record UpdateLocationRequest(
    string Name,
    AddressDTO Address,
    string TimeZone)
{
    public UpdateLocationCommand ToCommand(Guid id) =>
        new(id, Name, Address, TimeZone);
}