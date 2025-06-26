using SharedService.Core.Abstractions;

namespace DirectoryService.Application.Commands.Locations.SoftDeleteLocation;

public record SoftDeleteLocationCommand(
    Guid Id) : ICommand;