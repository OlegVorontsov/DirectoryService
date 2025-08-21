using DirectoryService.Web;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
var configuration = builder.Configuration;

services.AddProgramDependencies(configuration);

var app = builder.Build();
await app.Configure();

app.Run();

namespace DirectoryService.Web
{
    public partial class Program;
}