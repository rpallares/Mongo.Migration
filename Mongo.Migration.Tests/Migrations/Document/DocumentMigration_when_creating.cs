﻿using FluentAssertions;
using Mongo.Migration.Tests.TestDoubles;
using NUnit.Framework;

namespace Mongo.Migration.Tests.Migrations.Document;

[TestFixture]
public class DocumentMigrationWhenCreating
{
    [Test]
    public void Then_migration_has_type_testClass()
    {
        // Arrange Act
        var migration = new TestDocumentWithOneMigration001();

        // Assert
        migration.Type.Should().Be(typeof(TestDocumentWithOneMigration));
    }

    [Test]
    public void Then_migration_have_version()
    {
        // Arrange Act
        var migration = new TestDocumentWithOneMigration001();

        // Assert
        migration.Version.Should().Be("0.0.1");
    }

    [Test]
    public void Then_migration_should_be_created()
    {
        // Arrange Act
        var migration = new TestDocumentWithOneMigration001();

        // Assert
        migration.Should().BeOfType<TestDocumentWithOneMigration001>();
    }
}