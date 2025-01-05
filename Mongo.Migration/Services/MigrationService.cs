using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Mongo.Migration.Documents;
using Mongo.Migration.Documents.Serializers;
using Mongo.Migration.Migrations.Database;
using Mongo.Migration.Migrations.Document;
using Mongo.Migration.Services.Interceptors;
using Mongo.Migration.Startup.DotNetCore;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace Mongo.Migration.Services;

internal class MigrationService : IMigrationService
{
    private readonly ILogger<MigrationService> _logger;
    private readonly IMongoClient _client;
    private readonly MongoMigrationStartupSettings _startupSettings;
    private readonly IServiceProvider _provider;

    public MigrationService(
        ILogger<MigrationService> logger,
        IMongoClient client,
        MongoMigrationStartupSettings startupSettings,
        IServiceProvider provider)
    {
        _logger = logger;
        _client = client;
        _startupSettings = startupSettings;
        _provider = provider;
    }

    public void RegisterBsonStatics()
    {
        BsonSerializer.RegisterSerializer(new DocumentVersionSerializer());

        if (_startupSettings.RuntimeMigrationEnabled)
        {
            MigrationBsonSerializerProvider migrationSerializerProvider = _provider.GetRequiredService<MigrationBsonSerializerProvider>();
            BsonSerializer.RegisterSerializationProvider(migrationSerializerProvider);
        }
    }

    public async Task MigrateAsync(string databaseName, string? targetDatabaseVersion)
    {
        if (_startupSettings.DatabaseMigrationEnabled)
        {
            await ExecuteDatabaseMigrationAsync(databaseName, targetDatabaseVersion);
        }

        if (_startupSettings.StartupDocumentMigrationEnabled)
        {
            await ExecuteDocumentMigrationAsync(databaseName);
        }
    }

    private async Task ExecuteDatabaseMigrationAsync(string databaseName, string? targetVersion)
    {
        Stopwatch sw = Stopwatch.StartNew();

        DocumentVersion? typedTargetVersion = null;

        if (targetVersion is not null)
        {
            typedTargetVersion = DocumentVersion.Parse(targetVersion.AsSpan());
        }

        _logger.LogInformation("Executing database migration...");
        await using var scope = _provider.CreateAsyncScope();

        IMongoDatabase database = _client.GetDatabase(databaseName);
        IDatabaseMigrationRunner runner = scope.ServiceProvider.GetRequiredService<IDatabaseMigrationRunner>();
        runner.Run(database, typedTargetVersion);

        _logger.LogInformation("Database migration done in {ElapsedMs} ms", sw.ElapsedMilliseconds);
    }

    private async Task ExecuteDocumentMigrationAsync(string databaseName)
    {
        Stopwatch sw = Stopwatch.StartNew();
        _logger.LogInformation("Executing document migration...");
        await using var scope = _provider.CreateAsyncScope();

        IMongoDatabase database = _client.GetDatabase(databaseName);
        IStartUpDocumentMigrationRunner runner = scope.ServiceProvider.GetRequiredService<IStartUpDocumentMigrationRunner>();
        runner.RunAll(database);

        _logger.LogInformation("Database migration done in {ElapsedMs} ms", sw.ElapsedMilliseconds);
    }
}