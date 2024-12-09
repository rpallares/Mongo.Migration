using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Mongo.Migration.Services;
using Mongo.Migration.Startup.DotNetCore;
using MongoDB.Driver;
using NUnit.Framework;
using Testcontainers.MongoDb;

namespace Mongo.Migration.Tests;

[SetUpFixture]
public sealed class TestcontainersContext
{
    private static readonly MongoDbContainer s_mongoDbContainer = new MongoDbBuilder().Build();

    private static ServiceProvider? s_provider;

    public static IServiceProvider Provider => s_provider ?? throw new InvalidOperationException("Must be setup");

    public static IMongoClient MongoClient => Provider.GetRequiredService<IMongoClient>();


    [OneTimeSetUp]
    public async Task OneTimeSetup()
    {
        await s_mongoDbContainer.StartAsync();

        IServiceCollection services = new ServiceCollection();
        services
            .AddLogging(builder => builder.AddProvider(NullLoggerProvider.Instance))
            .AddSingleton<IMongoClient>(new MongoClient(s_mongoDbContainer.GetConnectionString()))
            .AddMigration();

        s_provider = services.BuildServiceProvider();

        IMigrationService migrationService = s_provider.GetRequiredService<IMigrationService>();
        migrationService.RegisterBsonStatics();
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        if (s_provider is not null)
        {
            await s_provider.DisposeAsync();
        }
        
        await s_mongoDbContainer.StopAsync();
        await s_mongoDbContainer.DisposeAsync();
    }
}