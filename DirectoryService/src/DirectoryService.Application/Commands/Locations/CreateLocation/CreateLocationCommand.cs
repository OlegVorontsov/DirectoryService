using DirectoryService.Application.Shared.DTOs;
using SharedService.Core.Abstractions;

namespace DirectoryService.Application.Commands.Locations.CreateLocation;

public record CreateLocationCommand(
    string Name,
    AddressDTO Address,
    string TimeZone) : ICommand;