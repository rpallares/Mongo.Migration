using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Mongo.Migration.Migrations.Document;
using Mongo.Migration.Tests.TestDoubles;
using MongoDB.Bson;
using NUnit.Framework;

namespace Mongo.Migration.Tests.Migrations.Document
{
    [TestFixture]
    internal class DocumentMigrationRunner_when_migrating_up : IntegrationTest
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
        public void When_migrate_up_the_lowest_version_Then_all_migrations_are_used()
        {
            // Arrange
            BsonDocument document = new()
            {
                { "Version", "0.0.0" },
                { "Dors", 3 }
            };

            // Act
            _runner.Run(typeof(TestDocumentWithTwoMigrationHighestVersion), document);

            // Assert
            document.Names.ToList()[1].Should().Be("Door");
            document.Values.ToList()[0].AsString.Should().Be("0.0.2");
        }

        [Test]
        public void When_document_has_no_version_Then_all_migrations_are_used()
        {
            // Arrange
            BsonDocument document = new()
            {
                { "Dors", 3 }
            };

            // Act
            _runner.Run(typeof(TestDocumentWithTwoMigrationHighestVersion), document);

            // Assert
            document.Names.ToList()[1].Should().Be("Door");
            document.Values.ToList()[0].AsString.Should().Be("0.0.2");
        }

        [Test]
        public void When_document_has_current_version_Then_nothing_happens()
        {
            // Arrange
            BsonDocument document = new()
            {
                { "Version", "0.0.2" },
                { "Door", 3 }
            };

            // Act
            _runner.Run(typeof(TestDocumentWithTwoMigrationHighestVersion), document);

            // Assert
            document.Names.ToList()[1].Should().Be("Door");
            document.Values.ToList()[0].AsString.Should().Be("0.0.2");
        }
    }
}