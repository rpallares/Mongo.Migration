using Microsoft.Extensions.Logging.Abstractions;
using Mongo.Migration.Documents;
using Mongo.Migration.Migrations.Locators;
using Mongo.Migration.Tests.TestDoubles;
using NUnit.Framework;

namespace Mongo.Migration.Tests.Migrations.Locators;

[TestFixture]
public class TypeMigrationLocatorWhenLocate
{
    private TypeMigrationLocator _locator;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        // Arrange
        _locator = new TypeMigrationLocator(NullLogger<TypeMigrationLocator>.Instance);
    }

    [Test]
    public void When_document_has_one_migration_Then_migrations_count_should_be_one()
    {
        // Act
        var result = _locator.GetMigrations(typeof(TestDocumentWithOneMigration));

        // Assert
        Assert.That(result, Has.Count.EqualTo(1));
    }

    [Test]
    public void When_document_has_two_migration_Then_migrations_count_should_be_two()
    {
        // Act
        var result = _locator.GetMigrations(typeof(TestDocumentWithTwoMigration));

        // Assert
        Assert.That(result, Has.Count.EqualTo(2));
    }

    [Test]
    public void When_get_latest_version_of_migrations()
    {
        // Act
        var version = _locator.GetLatestVersion(typeof(TestDocumentWithTwoMigration));

        // Assert
        Assert.That(version.ToString(), Is.EqualTo("0.0.2"));
    }

    [Test]
    public void When_get_migrations_from_to()
    {
        // Act
        var result = _locator
            .GetMigrationsFromTo(typeof(TestDocumentWithTwoMigration), DocumentVersion.Default, DocumentVersion.Parse("0.0.1"))
            .ToList();
        // Assert
        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result[0], Is.TypeOf<TestDocumentWithTwoMigration001>());
    }

    [Test]
    public void When_get_migrations_from_to_down()
    {
        // Act
        var result = _locator
            .GetMigrationsFromToDown(typeof(TestDocumentWithTwoMigration), DocumentVersion.Parse("0.0.2"), DocumentVersion.Parse("0.0.1"))
            .ToList();
        // Assert
        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result[0], Is.TypeOf<TestDocumentWithTwoMigration002>());
    }
}