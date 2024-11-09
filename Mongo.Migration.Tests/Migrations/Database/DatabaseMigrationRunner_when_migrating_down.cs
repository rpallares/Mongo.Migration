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
    private IDatabaseMigrationRunner? _runner;

    protected override void OnSetUp(DocumentVersion databaseMigrationVersion)
    {
        base.OnSetUp(databaseMigrationVersion);

        _runner = Provider.GetRequiredService<IDatabaseMigrationRunner>();
    }

    [TearDown]
    public void TearDown()
    {
        Dispose();
    }

    [Test]
    public void When_database_has_migrations_Then_down_all_migrations()
    {
        OnSetUp(DocumentVersion.Default());

        // Arrange
        InsertMigrations(
            new DatabaseMigration[]
            {
                new TestDatabaseMigration001(),
                new TestDatabaseMigration002(),
                new TestDatabaseMigration003()
            });

        // Act
        _runner?.Run(Db);

        // Assert
        var migrations = GetMigrationHistory();
        migrations.Should().BeEmpty();
    }

    [Test]
    public void When_database_has_migrations_Then_down_to_selected_migration()
    {
        OnSetUp(new("0.0.1"));

        // Arrange
        InsertMigrations(
            new DatabaseMigration[]
            {
                new TestDatabaseMigration001(),
                new TestDatabaseMigration002(),
                new TestDatabaseMigration003()
            });

        // Act
        _runner?.Run(Db);

        // Assert
        var migrations = GetMigrationHistory();
        migrations.Should().NotBeEmpty();
        migrations.Should().OnlyContain(m => m.Version == "0.0.1");
    }
}