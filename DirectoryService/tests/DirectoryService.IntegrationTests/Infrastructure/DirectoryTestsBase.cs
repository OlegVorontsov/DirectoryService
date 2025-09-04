using AutoFixture;
using DirectoryService.Infrastructure.DataBase.Write;
using DirectoryService.IntegrationTests.Seeding;
using Microsoft.Extensions.DependencyInjection;

namespace DirectoryService.IntegrationTests.Infrastructure;

public class DirectoryTestsBase : IClassFixture<DirectoryTestWebFactory>, IAsyncLifetime
{
    protected readonly DirectoryTestWebFactory Factory;
    protected readonly ApplicationWriteDBContext DbContext;
    protected readonly IServiceScope Scope;
    protected readonly Fixture Fixture;
    protected readonly TestDataSeeder Seeder;

    private readonly Func<Task> _resetDatabase;

    protected DirectoryTestsBase(DirectoryTestWebFactory factory)
    {
        _resetDatabase = factory.ResetDatabaseAsync;
        Scope = factory.Services.CreateScope();
        DbContext = Scope.ServiceProvider.GetRequiredService<ApplicationWriteDBContext>();
        Fixture = new Fixture();
        Factory = factory;

        Seeder = new TestDataSeeder(DbContext);
    }

    public Task InitializeAsync() => Task.CompletedTask;

    async Task IAsyncLifetime.DisposeAsync()
    {
        await _resetDatabase();
        Scope.Dispose();
    }
}