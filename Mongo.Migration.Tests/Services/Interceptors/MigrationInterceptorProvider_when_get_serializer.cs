using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Mongo.Migration.Services.Interceptors;
using Mongo.Migration.Tests.TestDoubles;
using NUnit.Framework;

namespace Mongo.Migration.Tests.Services.Interceptors;

[TestFixture]
internal class MigrationInterceptorProviderWhenGetSerializer : IntegrationTest
{
    [Test]
    public void When_entity_is_document_Then_provide_serializer()
    {
        // Arrange 
        var provider = Provider.GetRequiredService<IMigrationInterceptorProvider>();

        // Act
        var serializer = provider.GetSerializer(typeof(TestDocumentWithOneMigration));

        // Assert
        serializer.ValueType.Should().Be(typeof(TestDocumentWithOneMigration));
    }

    [Test]
    public void When_entity_is_not_document_Then_provide_null()
    {
        // Arrange 
        var provider = Provider.GetRequiredService<IMigrationInterceptorProvider>();

        // Act
        var serializer = provider.GetSerializer(typeof(TestClass));

        // Assert
        serializer.Should().BeNull();
    }
}