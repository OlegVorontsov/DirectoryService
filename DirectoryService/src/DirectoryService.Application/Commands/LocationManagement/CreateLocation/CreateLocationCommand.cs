using DirectoryService.Application.Shared.DTOs;
using SharedService.Core.Abstractions;

namespace DirectoryService.Application.Commands.LocationManagement.CreateLocation;

public record CreateLocationCommand(
    string Name,
    AddressDTO Address,
    string TimeZone) : ICommand;