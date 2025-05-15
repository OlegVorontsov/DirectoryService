using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharedService.Core.Abstractions;

namespace DirectoryService.Application;

public static class ApplicationDependencyInjection
{
    private static readonly Assembly _assembly = Assembly.GetExecutingAssembly();

    public static IServiceCollection AddApplication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddHandlers(_assembly);

        return services;
    }
}