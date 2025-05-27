using DirectoryService.Infrastructure.DataBase;
using Microsoft.EntityFrameworkCore;
using SharedService.Framework.Middlewares;

namespace DirectoryService.Web;

public static class AppConfiguration
{
    public static async Task<WebApplication> Configure(this WebApplication app)
    {
        app.UseStaticFiles();
        app.UseExceptionMiddleware();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/openapi/v1.json", "DirectoryService");
                options.InjectStylesheet("/swagger-ui/SwaggerDark.css");
            });
            await app.ApplyMigrations();
        }

        app.MapControllers();

        return app;
    }

    private static async Task ApplyMigrations(this WebApplication application)
    {
        await using var scope = application.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();
        await dbContext.Database.MigrateAsync();
    }
}