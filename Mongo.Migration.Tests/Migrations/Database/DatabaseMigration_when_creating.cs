using FluentAssertions;
using Mongo.Migration.Migrations.Database;
using Mongo.Migration.Tests.TestDoubles.Database;
using NUnit.Framework;

namespace Mongo.Migration.Tests.Migrations.Database;

[TestFixture]
public class DatabaseMigrationWhenCreating
{
    [Test]
    public void Then_migration_has_type_DatabaseMigration()
    {
        // Arrange Act
        var migration = new TestDatabaseMigration001();

        // Assert
        migration.Type.Should().Be(typeof(DatabaseMigration));
    }

    [Test]
    public void Then_migration_have_version()
    {
        // Arrange Act
        var migration = new TestDatabaseMigration001();

        // Assert
        migration.Version.ToString().Should().Be("0.0.1");
    }

    [Test]
    public void Then_migration_should_be_created()
    {
        // Arrange Act
        var migration = new TestDatabaseMigration001();

        // Assert
        migration.Should().BeOfType<TestDatabaseMigration001>();
    }
}