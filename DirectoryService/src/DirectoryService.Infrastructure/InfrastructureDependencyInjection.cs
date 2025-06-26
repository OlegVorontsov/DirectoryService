using DirectoryService.Application.Interfaces.DataBase;
using DirectoryService.Application.Interfaces.Repositories;
using DirectoryService.Application.Shared.Interfaces;
using DirectoryService.Infrastructure.DataBase;
using DirectoryService.Infrastructure.DataBase.Read;
using DirectoryService.Infrastructure.DataBase.Repositories;
using DirectoryService.Infrastructure.DataBase.Write;
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
}