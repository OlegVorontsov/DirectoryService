using SharedService.Core.Abstractions;

namespace DirectoryService.Application.Commands.PositionManagement.SoftDeletePosition;

public record SoftDeletePositionCommand(
    Guid Id) : ICommand;