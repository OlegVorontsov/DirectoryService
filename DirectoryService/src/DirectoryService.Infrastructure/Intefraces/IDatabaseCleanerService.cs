namespace DirectoryService.Infrastructure.Intefraces;

public interface IDatabaseCleanerService
{
    Task CleanTablesAsync(
        TimeSpan timeToRestore,
        CancellationToken stoppingToken = default);
}