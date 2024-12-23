using System.Collections.Immutable;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Mongo.Migration.Documents;

namespace Mongo.Migration.Migrations.Locators;

public abstract class MigrationLocator<TMigrationType> : IMigrationLocator<TMigrationType>
    where TMigrationType : class, IMigration
{
    private readonly ILogger<MigrationLocator<TMigrationType>> _logger;

    private readonly List<Assembly> _assemblies;

    private IDictionary<Type, IReadOnlyCollection<TMigrationType>>? _migrations;

    protected MigrationLocator(ILogger<MigrationLocator<TMigrationType>> logger)
    {
        _logger = logger;
        _assemblies = GetAssemblies();
    }

    protected IEnumerable<Assembly> Assemblies => _assemblies;

    protected virtual IDictionary<Type, IReadOnlyCollection<TMigrationType>> Migrations
    {
        get
        {
            if (_migrations is null)
            {
                Locate();
            }

            if (_migrations is null || _migrations.Count <= 0)
            {
                _logger.LogWarning("No migration found");
            }

            return _migrations ?? ImmutableDictionary<Type, IReadOnlyCollection<TMigrationType>>.Empty;
        }
        set => _migrations = value;
    }

    public IReadOnlyCollection<TMigrationType> GetMigrations(Type type)
    {
        if(Migrations.TryGetValue(type, out var migrations))
        {
            return migrations;
        }

        return Array.Empty<TMigrationType>();
    }

    public IEnumerable<TMigrationType> GetMigrationsFromTo(Type type, DocumentVersion version, DocumentVersion otherVersion)
    {
        return GetMigrations(type)
            .Where(m => m.Version > version && m.Version <= otherVersion);
    }

    public IEnumerable<TMigrationType> GetMigrationsFromToDown(Type type, DocumentVersion version, DocumentVersion otherVersion)
    {
        return GetMigrations(type)
            .Where(m => m.Version <= version && m.Version > otherVersion)
            .Reverse();
    }

    public DocumentVersion GetLatestVersion(Type type)
    {
        var migrations = GetMigrations(type);

        return migrations.Count > 0
            ? migrations.Max(m => m.Version)
            : DocumentVersion.Default;
    }

    public abstract void Locate();

    private static List<Assembly> GetAssemblies()
    {
        var location = AppDomain.CurrentDomain.BaseDirectory;
        var path = Path.GetDirectoryName(location);

        if (string.IsNullOrWhiteSpace(path))
        {
            throw new DirectoryNotFoundException(location);
        }

        var assemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
        var migrationAssemblies = Directory.GetFiles(path, "*.MongoMigrations*.dll").Select(Assembly.LoadFile);

        assemblies.AddRange(migrationAssemblies);

        return assemblies;
    }
}