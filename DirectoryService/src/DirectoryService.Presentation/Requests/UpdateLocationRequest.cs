using DirectoryService.Application.Commands.LocationManagement.UpdateLocation;
using DirectoryService.Application.Shared.DTOs;

namespace DirectoryService.Presentation.Requests;

public record UpdateLocationRequest(
    string Name,
    AddressDTO Address,
    string TimeZone)
{
    public UpdateLocationCommand ToCommand(Guid id) =>
        new(id, Name, Address, TimeZone);
}