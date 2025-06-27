using DirectoryService.Infrastructure.Intefraces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DirectoryService.Infrastructure.BackgroundServices;

public class SoftDeleteCleanerBackgroundService : BackgroundService
{
    private readonly ILogger<SoftDeleteCleanerBackgroundService> _logger;
    private readonly IDatabaseCleanerService _cleanerService;

    private readonly TimeSpan _checkPeriod;
    private readonly TimeSpan _timeToRestore;

    public SoftDeleteCleanerBackgroundService(
        ILogger<SoftDeleteCleanerBackgroundService> logger,
        IDatabaseCleanerService cleanerService,
        IOptions<SoftDeleteCleanerOptions> options)
    {
        _logger = logger;
        _cleanerService = cleanerService;

        if (options != null)
        {
            _checkPeriod = TimeSpan.FromHours(options.Value.CheckPeriodHours);
            _timeToRestore = TimeSpan.FromHours(options.Value.TimeToRestoreHours);
        }

        _checkPeriod = TimeSpan.FromSeconds(10);
        _timeToRestore = TimeSpan.FromSeconds(10);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("SoftDeleteCleanerBackgroundService started");

        using PeriodicTimer timer = new(_checkPeriod);

        // when stop requested, waiting for timer to tick will be cancelled too
        while (!stoppingToken.IsCancellationRequested &&
               await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                await _cleanerService.CleanTablesAsync(
                    _timeToRestore,
                    stoppingToken);

                _logger.LogInformation("SoftDeleteCleanerBackgroundService executed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }

        _logger.LogInformation("SoftDeleteCleanerBackgroundService finished");
    }

    public class SoftDeleteCleanerOptions
    {
        public required float CheckPeriodHours { get; set; }
        public required float TimeToRestoreHours { get; set; }
    }
}