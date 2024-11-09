using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Mongo.Migration.Startup;
using Mongo.Migration.Startup.DotNetCore;
using Mongo2Go;
using MongoDB.Driver;

namespace Mongo.Migration.Tests;

public class IntegrationTest : IDisposable
{
    private MongoClient? _client;

    private MongoDbRunner? _mongoToGoRunner;

    private ServiceProvider? _serviceProvider;

    protected IServiceProvider Provider => _serviceProvider ?? throw new InvalidOperationException("Must be setup");

    public void Dispose()
    {
        _serviceProvider?.Dispose();
        _mongoToGoRunner?.Dispose();
        GC.SuppressFinalize(this);
    }

    protected void OnSetUp()
    {
        _mongoToGoRunner = MongoDbRunner.Start();
        _client = new MongoClient(_mongoToGoRunner.ConnectionString);

        _client.GetDatabase("PerformanceTest").CreateCollection("Test");

        ServiceCollection serviceCollection = new();
        serviceCollection
            .AddLogging(l => l.AddProvider(NullLoggerProvider.Instance))
            .AddSingleton<IMongoClient>(_client)
            .AddMigration(new MongoMigrationSettings { ConnectionString = _mongoToGoRunner.ConnectionString, Database = "PerformanceTest" });

        _serviceProvider = serviceCollection.BuildServiceProvider();
    }
}