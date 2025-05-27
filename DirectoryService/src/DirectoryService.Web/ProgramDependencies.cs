using DirectoryService.Application;
using DirectoryService.Infrastructure;
using Microsoft.OpenApi.Models;
using SharedService.SharedKernel.Errors;
using SharedService.SharedKernel.Models;

namespace DirectoryService.Web;

public static class ProgramDependencies
{
    public static IServiceCollection AddProgramDependencies(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddProgramOpenApi();
        services.AddControllers();

        // register modules
        services.AddWeb(configuration)
                .AddApplication(configuration)
                .AddInfrastructure(configuration);

        return services;
    }

    private static void AddProgramOpenApi(this IServiceCollection services)
    {
        services.AddOpenApi(options =>
        {
            options.AddSchemaTransformer((schema, context, _) =>
            {
                if (context.JsonTypeInfo.Type == typeof(Envelope<ErrorList>))
                {
                    if (schema.Properties.TryGetValue("errors", out var errorsProp))
                    {
                        errorsProp.Items.Reference = new OpenApiReference
                        {
                            Type = ReferenceType.Schema,
                            Id = "Error",
                        };
                    }
                }

                return Task.CompletedTask;
            });
        });
    }
}