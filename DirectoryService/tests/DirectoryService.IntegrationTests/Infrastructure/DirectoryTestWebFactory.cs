using System.Data.Common;
using DirectoryService.Infrastructure.DataBase.Write;
using DirectoryService.Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Npgsql;
using Respawn;
using Testcontainers.PostgreSql;

namespace DirectoryService.IntegrationTests.Infrastructure;

public class DirectoryTestWebFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres")
        .WithDatabase("DirectoryService_Test")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    private Respawner _respawner;
    private DbConnection _dbConnection;
    private ApplicationWriteDBContext _context;

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();

        _dbConnection = new NpgsqlConnection(_dbContainer.GetConnectionString());
        _context = Services
            .CreateScope().ServiceProvider.GetRequiredService<ApplicationWriteDBContext>();
        await _context.Database.EnsureCreatedAsync();

        await InitializeRespawner();

        // using IServiceScope scope = Services.CreateScope();
        // var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationWriteDBContext>();

        // await dbContext.Database.EnsureDeletedAsync();
        // await dbContext.Database.EnsureCreatedAsync();

        await _context.Database.ExecuteSqlRawAsync(
            "CREATE INDEX IF NOT EXISTS departments_path_gist_idx ON directory_service.departments USING GIST (path);");

        // _dbConnection = new NpgsqlConnection(_dbContainer.GetConnectionString());
        // await InitializeRespawner();
    }

    public async Task ResetDatabaseAsync()
    {
        await _respawner.ResetAsync(_dbConnection);
    }

    public new async Task DisposeAsync()
    {
        await _dbContainer.StopAsync();
        await _dbContainer.DisposeAsync();

        await _dbConnection.DisposeAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(ConfigureDefaultServices);
    }

    protected virtual void ConfigureDefaultServices(IServiceCollection services)
    {
        services.RemoveAll<ApplicationWriteDBContext>();

        // Add test database services
        services.AddScoped(_ =>
            new ApplicationWriteDBContext(_dbContainer.GetConnectionString()));
    }

    private async Task InitializeRespawner()
    {
        await _dbConnection.OpenAsync();

        _respawner = await Respawner.CreateAsync(
            _dbConnection,
            new RespawnerOptions
            {
                DbAdapter = DbAdapter.Postgres,
                SchemasToInclude = ["public"],
            });
    }
}