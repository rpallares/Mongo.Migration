using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Mongo.Migration.Startup;
using Mongo.Migration.Startup.DotNetCore;
using MongoDB.Driver;
using NUnit.Framework;

namespace Mongo.Migration.Tests;

[TestFixture]
public class IntegrationTest
{
    private const string DatabaseName = "PerformanceTest";
    private const string CollectionName = "Test";
    private ServiceProvider? _serviceProvider;

    protected IServiceProvider Provider => _serviceProvider ?? throw new InvalidOperationException("Must be setup");

    [SetUp]
    protected async Task SetUpAsync()
    {
        IMongoClient client = TestcontainersContext.MongoClient;
        await client.GetDatabase(DatabaseName).CreateCollectionAsync(CollectionName);

        ServiceCollection serviceCollection = new();
        serviceCollection
            .AddLogging(l => l.AddProvider(NullLoggerProvider.Instance))
            .AddSingleton<IMongoClient>(client)
            .AddMigration(new MongoMigrationSettings { Database = DatabaseName });

        _serviceProvider = serviceCollection.BuildServiceProvider();
    }

    [TearDown]
    protected async Task TearDownAsync()
    {
        if (_serviceProvider is not null)
        {
            await _serviceProvider.DisposeAsync();
        }
        IMongoClient client = TestcontainersContext.MongoClient;
        await client.GetDatabase(DatabaseName)
            .DropCollectionAsync(CollectionName);
    }
}