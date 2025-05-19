using SharedService.Framework.Middlewares;

namespace DirectoryService.Web;

public static class AppConfiguration
{
    public static WebApplication Configure(this WebApplication app)
    {
        app.UseExceptionMiddleware();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "DirectoryService"));
        }

        app.MapControllers();

        return app;
    }
}