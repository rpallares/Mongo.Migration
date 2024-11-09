using System.Reflection;
using Microsoft.Extensions.Logging;
using Mongo.Migration.Documents;
using Mongo.Migration.Exceptions;

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
                _logger.LogInformation(new NoMigrationsFoundException(), "No migration found");
            }

            return _migrations!;
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
        var migrations = GetMigrations(type);

        return
            migrations
                .Where(m => m.Version > version)
                .Where(m => m.Version <= otherVersion)
                .ToList();
    }

    public IEnumerable<TMigrationType> GetMigrationsGt(Type type, DocumentVersion version)
    {
        var migrations = GetMigrations(type);

        return
            migrations
                .Where(m => m.Version > version)
                .ToList();
    }

    public IEnumerable<TMigrationType> GetMigrationsGtEq(Type type, DocumentVersion version)
    {
        var migrations = GetMigrations(type);

        return
            migrations
                .Where(m => m.Version >= version)
                .ToList();
    }

    public DocumentVersion GetLatestVersion(Type type)
    {
        var migrations = GetMigrations(type);

        return migrations.Count > 0
            ? migrations.Max(m => m.Version)
            : DocumentVersion.Default();
    }

    public abstract void Locate();

    private static List<Assembly> GetAssemblies()
    {
        var location = AppDomain.CurrentDomain.BaseDirectory;
        var path = Path.GetDirectoryName(location);

        if (string.IsNullOrWhiteSpace(path))
        {
            throw new DirectoryNotFoundException(ErrorTexts.AppDirNotFound);
        }

        var assemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
        var migrationAssemblies = Directory.GetFiles(path, "*.MongoMigrations*.dll").Select(Assembly.LoadFile);

        assemblies.AddRange(migrationAssemblies);

        return assemblies;
    }
}