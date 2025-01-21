using Microsoft.Extensions.DependencyInjection;
using Mongo.Migration.Bson;
using Mongo.Migration.Documents.Locators;
using Mongo.Migration.Migrations.Database;
using Mongo.Migration.Migrations.Document;
using Mongo.Migration.Migrations.Locators;
using Mongo.Migration.Services;

namespace Mongo.Migration.Startup;

public class MongoMigrationBuilder
{
    private readonly IServiceCollection _services;
    private readonly MongoMigrationSettings _mongoMigrationSettings;
    private readonly MongoMigrationStartupSettings _mongoMigrationStartupSettings;

    private bool _interceptorAdded;

    internal MongoMigrationBuilder(IServiceCollection services)
    {
        _services = services;
        _mongoMigrationSettings = new MongoMigrationSettings();
        _mongoMigrationStartupSettings = new MongoMigrationStartupSettings();

        _services
            .AddSingleton(_mongoMigrationSettings)
            .AddSingleton(_mongoMigrationStartupSettings)
            .AddTransient<IMigrationService, MigrationService>();
    }

    public string VersionFieldName
    {
        get => _mongoMigrationSettings.VersionFieldName;
        set => _mongoMigrationSettings.VersionFieldName = value;
    }

    public MongoMigrationBuilder AddRuntimeDocumentMigration()
    {
        AddInterceptorServices();

        _services
            .AddSingleton<MigrationBsonSerializerProvider>();

        _mongoMigrationStartupSettings.RuntimeMigrationEnabled = true;
        return this;
    }

    public MongoMigrationBuilder AddStartupDocumentMigration()
    {
        AddInterceptorServices();

        _services
            .AddTransient<ICollectionLocator, CollectionLocator>()
            .AddTransient<IStartUpDocumentMigrationRunner, StartUpDocumentMigrationRunner>();

        _mongoMigrationStartupSettings.StartupDocumentMigrationEnabled = true;

        return this;
    }

    public MongoMigrationBuilder AddDatabaseMigration()
    {
        _services
            .AddTransient<IDatabaseTypeMigrationDependencyLocator, DatabaseTypeMigrationDependencyLocator>()
            .AddTransient<IDatabaseVersionService, DatabaseVersionService>()
            .AddTransient<IDatabaseMigrationRunner, DatabaseMigrationRunner>();

        _mongoMigrationStartupSettings.DatabaseMigrationEnabled = true;

        return this;
    }

    public void AddAllMigrationsIfNothingWasAdded()
    {
        if (_mongoMigrationStartupSettings is { RuntimeMigrationEnabled: false, StartupDocumentMigrationEnabled: false, DatabaseMigrationEnabled: false })
        {
            AddRuntimeDocumentMigration();
            AddDatabaseMigration();
            AddStartupDocumentMigration();
        }
    }

    private void AddInterceptorServices()
    {
        if (_interceptorAdded)
        {
            return;
        }

        _services
            .AddSingleton<IMigrationLocator<IDocumentMigration>, TypeMigrationLocator>()
            .AddSingleton<IRuntimeVersionLocator, RuntimeVersionLocator>()
            .AddSingleton<IStartUpVersionLocator, StartUpVersionLocator>()
            .AddSingleton<IDocumentVersionService, DocumentVersionService>()
            .AddSingleton<IDocumentMigrationRunner, DocumentMigrationRunner>();

        _interceptorAdded = true;
    }
}