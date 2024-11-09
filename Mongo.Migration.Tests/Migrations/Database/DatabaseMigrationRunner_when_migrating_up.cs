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
    private IDatabaseMigrationRunner _runner;

    [SetUp]
    public void SetUp()
    {
        base.OnSetUp(DocumentVersion.Empty());

        _runner = Provider.GetRequiredService<IDatabaseMigrationRunner>();
    }

    [TearDown]
    public void TearDown()
    {
        Dispose();
    }

    [Test]
    public void When_database_has_no_migrations_Then_all_migrations_are_used()
    {
        // Act
        _runner.Run(Db);

        // Assert
        var migrations = GetMigrationHistory();
        migrations.Should().NotBeEmpty();
        migrations[0].Version.ToString().Should().BeEquivalentTo("0.0.1");
        migrations[1].Version.ToString().Should().BeEquivalentTo("0.0.2");
        migrations[2].Version.ToString().Should().BeEquivalentTo("0.0.3");
    }

    [Test]
    public void When_database_has_migrations_Then_latest_migrations_are_used()
    {
        // Arrange
        InsertMigrations(new DatabaseMigration[] { new TestDatabaseMigration001(), new TestDatabaseMigration002() });

        // Act
        _runner.Run(Db);

        // Assert
        var migrations = GetMigrationHistory();
        migrations.Should().NotBeEmpty();
        migrations[2].Version.ToString().Should().BeEquivalentTo("0.0.3");
    }

    [Test]
    public void When_database_has_latest_version_Then_nothing_happens()
    {
        // Arrange
        InsertMigrations(
            new DatabaseMigration[] { new TestDatabaseMigration001(), new TestDatabaseMigration002(), new TestDatabaseMigration003() });

        // Act
        _runner.Run(Db);

        // Assert
        var migrations = GetMigrationHistory();
        migrations.Should().NotBeEmpty();
        migrations[0].Version.ToString().Should().BeEquivalentTo("0.0.1");
        migrations[1].Version.ToString().Should().BeEquivalentTo("0.0.2");
        migrations[2].Version.ToString().Should().BeEquivalentTo("0.0.3");
    }
}