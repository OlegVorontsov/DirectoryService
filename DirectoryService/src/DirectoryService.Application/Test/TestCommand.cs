using SharedService.Core.Abstractions;

namespace DirectoryService.Application.Test;

public record TestCommand(
    Guid TestId,
    string Comment) : ICommand;