using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Mongo.Migration.Documents;
using Mongo.Migration.Migrations.Database;
using Mongo.Migration.Tests.TestDoubles.Database;
using NUnit.Framework;

namespace Mongo.Migration.Tests.Migrations.Database;

[TestFixture]
internal class DatabaseMigrationRunnerWhenMigratingDown : DatabaseIntegrationTest
{
    [Test]
    public async Task When_database_has_migrations_Then_down_all_migrations()
    {
        await OnSetUpAsync();
        IDatabaseMigrationRunner runner = TestcontainersContext.Provider.GetRequiredService<IDatabaseMigrationRunner>();

        // Arrange
        InsertMigrations(
            new DatabaseMigration[]
            {
                new TestDatabaseMigration001(),
                new TestDatabaseMigration002(),
                new TestDatabaseMigration003()
            });

        // Act
        runner.Run(Db, DocumentVersion.Default);

        // Assert
        var migrations = GetMigrationHistory();
        migrations.Should().BeEmpty();
    }

    [Test]
    public async Task When_database_has_migrations_Then_down_to_selected_migration()
    {
        await OnSetUpAsync();
        IDatabaseMigrationRunner runner = TestcontainersContext.Provider.GetRequiredService<IDatabaseMigrationRunner>();

        // Arrange
        InsertMigrations(
            new DatabaseMigration[]
            {
                new TestDatabaseMigration001(),
                new TestDatabaseMigration002(),
                new TestDatabaseMigration003()
            });

        // Act
        runner.Run(Db, new DocumentVersion(0, 0, 1));

        // Assert
        var migrations = GetMigrationHistory();
        migrations.Should().NotBeEmpty();
        migrations.Should().OnlyContain(m => m.Version == new DocumentVersion(0, 0, 1));
    }
}