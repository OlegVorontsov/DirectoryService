using DirectoryService.Infrastructure.DataBase;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DirectoryService.Infrastructure;

public static class InfrastructureDependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDataBase(configuration);

        return services;
    }

    private static IServiceCollection AddDataBase(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<ApplicationDBContext>(_ =>
            new ApplicationDBContext(configuration.GetConnectionString(ApplicationDBContext.DATABASE_CONFIGURATION!)));

        return services;
    }
}