using System.Reflection;
using Serilog;
using Serilog.Events;

namespace DirectoryService.Web;

public static class WebDependencyInjection
{
    public static IServiceCollection AddWeb(this IServiceCollection services, IConfiguration config)
    {
        AddSerilogLogger(services, config);

        return services;
    }

    private static void AddSerilogLogger(this IServiceCollection services, IConfiguration config)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.Debug()
            .Enrich.WithThreadName()
            .WriteTo.Seq(config.GetConnectionString("Seq") ??
                         throw new ArgumentNullException("Seq connection string wasn't found"))
            .MinimumLevel.Override("Microsoft.AspNetCore.Hosting", LogEventLevel.Information)
            .MinimumLevel.Override("Microsoft.AspNetCore.Mvc", LogEventLevel.Information)
            .MinimumLevel.Override("Microsoft.AspNetCore.Routing", LogEventLevel.Information)
            .CreateLogger();

        services.AddSerilog();
    }
}