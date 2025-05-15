using DirectoryService.Application;
using Microsoft.OpenApi.Models;
using SharedService.SharedKernel.Errors;
using SharedService.SharedKernel.Models;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
var configuration = builder.Configuration;

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

services.AddControllers();

// register modules
services.AddApplication(configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "DirectoryService"));
}

app.MapControllers();
app.Run();