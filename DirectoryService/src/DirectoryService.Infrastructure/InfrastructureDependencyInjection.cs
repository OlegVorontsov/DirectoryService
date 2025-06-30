using DirectoryService.Application.Interfaces.DataBase;
using DirectoryService.Application.Interfaces.Repositories;
using DirectoryService.Application.Shared.Interfaces;
using DirectoryService.Infrastructure.BackgroundServices;
using DirectoryService.Infrastructure.DataBase;
using DirectoryService.Infrastructure.DataBase.Read;
using DirectoryService.Infrastructure.DataBase.Repositories;
using DirectoryService.Infrastructure.DataBase.Write;
using DirectoryService.Infrastructure.Intefraces;
using DirectoryService.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DirectoryService.Infrastructure;

public static class InfrastructureDependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDataBase(configuration)
                .AddBackgroundServices(configuration);

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
}