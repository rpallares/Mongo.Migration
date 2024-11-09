using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Mongo.Migration.Documents;
using Mongo.Migration.Services;
using MongoDB.Bson.Serialization;
using NUnit.Framework;

namespace Mongo.Migration.Tests.MongoDB;

[TestFixture]
internal class MongoRegistratorWhenRegistrating : IntegrationTest
{
    [Test]
    public void Then_serializer_is_registered()
    {
        // Arrange 
        var migrationService = Provider.GetRequiredService<IMigrationService>();

        // Act
        migrationService.Migrate();

        // Arrange
        BsonSerializer.LookupSerializer<DocumentVersion>().ValueType.Should().Be(typeof(DocumentVersion));
    }
}