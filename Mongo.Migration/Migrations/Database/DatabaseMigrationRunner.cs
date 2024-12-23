﻿using Microsoft.Extensions.Logging;
using Mongo.Migration.Documents;
using Mongo.Migration.Migrations.Locators;
using Mongo.Migration.Services;
using MongoDB.Driver;

namespace Mongo.Migration.Migrations.Database;

internal class DatabaseMigrationRunner : IDatabaseMigrationRunner
{
    private readonly IDatabaseVersionService _databaseVersionService;

    private readonly ILogger _logger;

    private readonly Type _databaseMigrationType = typeof(DatabaseMigration);

    private readonly IDatabaseTypeMigrationDependencyLocator _migrationLocator;

    public DatabaseMigrationRunner(
        IDatabaseTypeMigrationDependencyLocator migrationLocator,
        IDatabaseVersionService databaseVersionService,
        ILogger<DatabaseMigrationRunner> logger)
    {
        _migrationLocator = migrationLocator;
        _databaseVersionService = databaseVersionService;
        _logger = logger;
    }

    public void Run(IMongoDatabase db, in DocumentVersion? targetVersion = null)
    {
        _logger.LogInformation("Database migration started.");
        DocumentVersion databaseVersion = _databaseVersionService.GetLatestDatabaseVersion(db);
        DocumentVersion currentOrLatest = targetVersion is not null && targetVersion > DocumentVersion.Empty
            ? targetVersion.Value
            : _databaseVersionService.GetLatestMigrationVersion();

        if (databaseVersion != currentOrLatest)
        {
            MigrateUpOrDown(db, databaseVersion, currentOrLatest);
        }
        
        _logger.LogInformation("Database migration finished.");
    }

    private void MigrateUpOrDown(
        IMongoDatabase db,
        in DocumentVersion databaseVersion,
        in DocumentVersion to)
    {
        if (databaseVersion > to)
        {
            MigrateDown(db, databaseVersion, to);
            return;
        }

        MigrateUp(db, databaseVersion, to);
    }

    private void MigrateUp(IMongoDatabase db, in DocumentVersion currentVersion, in DocumentVersion toVersion)
    {
        var migrations = _migrationLocator
            .GetMigrationsFromTo(_databaseMigrationType, currentVersion, toVersion);

        foreach (var migration in migrations)
        {
            _logger.LogInformation("Database Migration Up: {Type}:{Version} ", currentVersion.GetType(), migration.Version);

            migration.Up(db);
            _databaseVersionService.Save(db, migration);

            _logger.LogInformation("Database Migration Up finished successful: {Type}:{Version} ", migration.GetType(), migration.Version);
        }
    }

    private void MigrateDown(IMongoDatabase db, in DocumentVersion currentVersion, in DocumentVersion toVersion)
    {
        var migrations = _migrationLocator
            .GetMigrationsFromToDown(_databaseMigrationType, currentVersion, toVersion);

        foreach (var migration in migrations)
        {
            _logger.LogInformation("Database Migration Down: {Type}:{Version} ", migration.GetType(), migration.Version);

            migration.Down(db);
            _databaseVersionService.Remove(db, migration);

            _logger.LogInformation("Database Migration Down finished successful: {Type}:{Version} ", migration.GetType(), migration.Version);
        }
    }
}