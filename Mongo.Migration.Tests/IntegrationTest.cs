using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Mongo.Migration.Startup;
using Mongo.Migration.Startup.DotNetCore;
using Mongo2Go;
using MongoDB.Driver;

namespace Mongo.Migration.Tests
{
    public class IntegrationTest : IDisposable
    {
        protected IMongoClient _client;

        protected IServiceProvider _serviceProvider;

        protected MongoDbRunner _mongoToGoRunner;

        public void Dispose()
        {
            _mongoToGoRunner?.Dispose();
        }

        protected void OnSetUp()
        {
            _mongoToGoRunner = MongoDbRunner.Start();
            _client = new MongoClient(_mongoToGoRunner.ConnectionString);

            _client.GetDatabase("PerformanceTest").CreateCollection("Test");

            ServiceCollection serviceCollection = new ServiceCollection();
            serviceCollection
                .AddLogging(l => l.AddProvider(NullLoggerProvider.Instance))
                .AddSingleton<IMongoClient>(_client)
                .AddMigration(new MongoMigrationSettings { ConnectionString = _mongoToGoRunner.ConnectionString, Database = "PerformanceTest" });

            _serviceProvider = serviceCollection.BuildServiceProvider();
        }
    }
}