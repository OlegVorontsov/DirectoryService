using SharedService.Core.Abstractions;

namespace DirectoryService.Application.Commands.Positions.SoftDeletePosition;

public record SoftDeletePositionCommand(
    Guid Id) : ICommand;