using SharedService.Core.Abstractions;

namespace DirectoryService.Application.Commands.LocationManagement.SoftDeleteLocation;

public record SoftDeleteLocationCommand(
    Guid Id) : ICommand;