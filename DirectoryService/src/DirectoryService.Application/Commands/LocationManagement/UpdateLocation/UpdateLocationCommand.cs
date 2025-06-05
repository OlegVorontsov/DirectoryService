using DirectoryService.Application.Shared.DTOs;
using SharedService.Core.Abstractions;

namespace DirectoryService.Application.Commands.LocationManagement.UpdateLocation;

public record UpdateLocationCommand(
    Guid Id,
    string Name,
    AddressDTO Address,
    string TimeZone) : ICommand;