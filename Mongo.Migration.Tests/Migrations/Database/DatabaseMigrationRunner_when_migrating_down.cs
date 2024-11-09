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
        await OnSetUpAsync(DocumentVersion.Default());
        IDatabaseMigrationRunner runner = Provider.GetRequiredService<IDatabaseMigrationRunner>();

        // Arrange
        InsertMigrations(
            new DatabaseMigration[]
            {
                new TestDatabaseMigration001(),
                new TestDatabaseMigration002(),
                new TestDatabaseMigration003()
            });

        // Act
        runner.Run(Db);

        // Assert
        var migrations = GetMigrationHistory();
        migrations.Should().BeEmpty();
    }

    [Test]
    public async Task When_database_has_migrations_Then_down_to_selected_migration()
    {
        await OnSetUpAsync(new("0.0.1"));
        IDatabaseMigrationRunner runner = Provider.GetRequiredService<IDatabaseMigrationRunner>();

        // Arrange
        InsertMigrations(
            new DatabaseMigration[]
            {
                new TestDatabaseMigration001(),
                new TestDatabaseMigration002(),
                new TestDatabaseMigration003()
            });

        // Act
        runner.Run(Db);

        // Assert
        var migrations = GetMigrationHistory();
        migrations.Should().NotBeEmpty();
        migrations.Should().OnlyContain(m => m.Version == "0.0.1");
    }
}