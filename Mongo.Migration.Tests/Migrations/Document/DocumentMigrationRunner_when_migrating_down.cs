﻿using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Mongo.Migration.Migrations.Document;
using Mongo.Migration.Tests.TestDoubles;
using MongoDB.Bson;
using NUnit.Framework;

namespace Mongo.Migration.Tests.Migrations.Document;

[TestFixture]
internal class DocumentMigrationRunner_when_migrating_down : IntegrationTest
{
    private IDocumentMigrationRunner _runner;

    [SetUp]
    public void SetUp()
    {
        OnSetUp();

        _runner = Provider.GetRequiredService<IDocumentMigrationRunner>();
    }

    [TearDown]
    public void TearDown()
    {
        Dispose();
    }

    [Test]
    public void When_migrating_down_Then_all_migrations_are_used()
    {
        // Arrange
        BsonDocument document = new()
        {
            { "Version", "0.0.2" },
            { "Door", 3 }
        };

        // Act
        _runner.Run(typeof(TestDocumentWithTwoMigration), document);

        // Assert
        document.Names.ToList()[1].Should().Be("Dors");
        document.Values.ToList()[0].AsString.Should().Be("0.0.0");
    }

    [Test]
    public void When_document_has_Then_all_migrations_are_used_to_that_version()
    {
        // Arrange
        // Arrange
        BsonDocument document = new()
        {
            { "Version", "0.0.2" },
            { "Door", 3 }
        };

        // Act
        _runner.Run(typeof(TestDocumentWithTwoMigrationMiddleVersion), document);

        // Assert
        document.Names.ToList()[1].Should().Be("Doors");
        document.Values.ToList()[0].AsString.Should().Be("0.0.1");
    }
}