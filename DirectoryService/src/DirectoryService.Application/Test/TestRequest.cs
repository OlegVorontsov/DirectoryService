namespace DirectoryService.Application.Test;

public record TestRequest(string Comment)
{
    public TestCommand ToCommand(Guid testId) =>
        new(testId, Comment);
}