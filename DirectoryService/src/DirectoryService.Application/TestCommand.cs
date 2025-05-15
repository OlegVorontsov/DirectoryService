using SharedService.Core.Abstractions;

namespace DirectoryService.Application;

public record TestCommand(
    Guid TestId) : ICommand;