using FluentAssertions;
using Mongo.Migration.Tests.TestDoubles;
using MongoDB.Bson;
using NUnit.Framework;

namespace Mongo.Migration.Tests.Migrations.Document;

[TestFixture]
public class DocumentMigrationWhenMigrating
{
    [Test]
    public void When_migrating_down_Then_document_changes()
    {
        // Arrange
        var migration = new TestDocumentWithOneMigration001();
        var document = new BsonDocument { { "Doors", 3 } };

        // Act
        migration.Down(document);

        // Assert
        document.Should().BeEquivalentTo(new BsonDocument { { "Dors", 3 } });
    }

    [Test]
    public void When_migrating_up_Then_document_changes()
    {
        // Arrange
        var migration = new TestDocumentWithOneMigration001();
        var document = new BsonDocument { { "Dors", 3 } };

        // Act
        migration.Up(document);

        // Assert
        document.Should().BeEquivalentTo(new BsonDocument { { "Doors", 3 } });
    }
}