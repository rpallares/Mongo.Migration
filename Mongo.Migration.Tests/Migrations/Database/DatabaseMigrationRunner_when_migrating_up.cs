using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Mongo.Migration.Documents;
using Mongo.Migration.Migrations.Database;
using Mongo.Migration.Tests.TestDoubles.Database;
using NUnit.Framework;

namespace Mongo.Migration.Tests.Migrations.Database;

[TestFixture]
internal class DatabaseMigrationRunnerWhenMigratingUp : DatabaseIntegrationTest
{
    [Test]
    public async Task When_database_has_no_migrations_Then_all_migrations_are_used()
    {
        // Arrange
        await OnSetUpAsync();
        IDatabaseMigrationRunner runner = TestcontainersContext.Provider.GetRequiredService<IDatabaseMigrationRunner>();

        // Act
        runner.Run(Db, DocumentVersion.Empty);

        // Assert
        var migrations = GetMigrationHistory();
        migrations.Should().NotBeEmpty();
        migrations[0].Version.ToString().Should().BeEquivalentTo("0.0.1");
        migrations[1].Version.ToString().Should().BeEquivalentTo("0.0.2");
        migrations[2].Version.ToString().Should().BeEquivalentTo("0.0.3");
    }

    [Test]
    public async Task When_database_has_migrations_Then_latest_migrations_are_used()
    {
        // Arrange
        await OnSetUpAsync();
        IDatabaseMigrationRunner runner = TestcontainersContext.Provider.GetRequiredService<IDatabaseMigrationRunner>();
        InsertMigrations(new DatabaseMigration[] { new TestDatabaseMigration001(), new TestDatabaseMigration002() });

        // Act
        runner.Run(Db, DocumentVersion.Empty);

        // Assert
        var migrations = GetMigrationHistory();
        migrations.Should().NotBeEmpty();
        migrations[2].Version.ToString().Should().BeEquivalentTo("0.0.3");
    }

    [Test]
    public async Task When_database_has_latest_version_Then_nothing_happens()
    {
        // Arrange
        await OnSetUpAsync();
        IDatabaseMigrationRunner runner = TestcontainersContext.Provider.GetRequiredService<IDatabaseMigrationRunner>();
        InsertMigrations(new DatabaseMigration[] { new TestDatabaseMigration001(), new TestDatabaseMigration002(), new TestDatabaseMigration003() });

        // Act
        runner.Run(Db, DocumentVersion.Empty);

        // Assert
        var migrations = GetMigrationHistory();
        migrations.Should().NotBeEmpty();
        migrations[0].Version.ToString().Should().BeEquivalentTo("0.0.1");
        migrations[1].Version.ToString().Should().BeEquivalentTo("0.0.2");
        migrations[2].Version.ToString().Should().BeEquivalentTo("0.0.3");
    }
}