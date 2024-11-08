using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Mongo.Migration.Services.Interceptors;
using Mongo.Migration.Tests.TestDoubles;
using NUnit.Framework;

namespace Mongo.Migration.Tests.Services.Interceptors
{
    [TestFixture]
    internal class MigrationInterceptorFactory_when_creating : IntegrationTest
    {
        [SetUp]
        public void SetUp()
        {
            OnSetUp();
        }

        [TearDown]
        public void TearDown()
        {
            Dispose();
        }

        [Test]
        public void If_type_is_assignable_to_document_Then_interceptor_is_created()
        {
            // Arrange
            var factory = Provider.GetRequiredService<IMigrationInterceptorFactory>();

            // Act
            var interceptor = factory.Create(typeof(TestDocumentWithOneMigration));

            // Assert
            interceptor.ValueType.Should().Be<TestDocumentWithOneMigration>();
        }

        [Test]
        public void If_type_is_not_assignable_to_document_Then_exception_is_thrown()
        {
            // Arrange
            var factory = Provider.GetRequiredService<IMigrationInterceptorFactory>();

            // Act
            Action act = () => factory.Create(typeof(TestClass));

            // Assert
            act.Should().ThrowExactly<ArgumentException>();
        }

        [Test]
        public void If_type_is_null_Then_exception_is_thrown()
        {
            // Arrange
            var factory = Provider.GetRequiredService<IMigrationInterceptorFactory>();

            // Act
            Action act = () => factory.Create(null!);

            // Assert
            act.Should().ThrowExactly<ArgumentNullException>();
        }
    }
}