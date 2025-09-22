using DirectoryService.Application.Interfaces.Repositories;
using DirectoryService.Infrastructure.DataBase;
using DirectoryService.Infrastructure.DataBase.Read;
using DirectoryService.Infrastructure.DataBase.Repositories;
using DirectoryService.Infrastructure.DataBase.Write;
using DirectoryService.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharedService.Core.BackgroundServices;
using SharedService.Core.BackgroundServices.Intefraces;
using SharedService.Core.Database.Intefraces;
using SharedService.Core.Database.Read;

namespace DirectoryService.Infrastructure;

public static class InfrastructureDependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDataBase(configuration)
                .AddBackgroundServices(configuration)
                .AddDistributedCache(configuration);

        return services;
    }

    private static IServiceCollection AddDataBase(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        DapperConfigurationHelper.Configure();

        var dbConnectionString = configuration.GetConnectionString(ApplicationWriteDBContext.DATABASE_CONFIGURATION);
        services.AddScoped(_ => new ApplicationWriteDBContext(dbConnectionString));

        services.AddSingleton<IDBConnectionFactory>(_ => new ReadDBConnectionFactory(dbConnectionString));

        services.AddScoped<ILocationRepository, LocationRepository>();
        services.AddScoped<IDepartmentRepository, DepartmentRepository>();
        services.AddScoped<IPositionRepository, PositionRepository>();
        services.AddScoped<IDepartmentLocationRepository, DepartmentLocationRepository>();

        services.AddScoped<IUnitOfWork, DirectoryServiceUnitOfWork>();

        return services;
    }

    private static IServiceCollection AddBackgroundServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSingleton<IDatabaseCleanerService, DatabaseCleanerService>();

        var configs = configuration.GetSection("SoftDeleteCleaner");
        services.Configure<SoftDeleteCleanerBackgroundService.SoftDeleteCleanerOptions>(configs);
        services.AddHostedService<SoftDeleteCleanerBackgroundService>();

        return services;
    }

    private static IServiceCollection AddDistributedCache(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddStackExchangeRedisCache(options =>
        {
            string connection = configuration.GetConnectionString("Redis") ??
                                throw new ArgumentNullException(nameof(connection));

            options.Configuration = connection;
        });

        return services;
    }
}