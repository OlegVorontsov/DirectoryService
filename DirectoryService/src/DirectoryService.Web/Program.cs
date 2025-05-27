using DirectoryService.Web;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
var configuration = builder.Configuration;

services.AddProgramDependencies(configuration);

var app = builder.Build();
await app.Configure();

app.Run();