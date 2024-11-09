using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Mongo.Migration.Documents;
using Mongo.Migration.Services;
using Mongo.Migration.Tests.Migrations.Database;
using NUnit.Framework;

namespace Mongo.Migration.Tests.Services;

[TestFixture]
internal class DatabaseVersionServiceWhenDetermineVersion : DatabaseIntegrationTest
{
    [Test]
    public async Task When_project_has_migrations_Then_get_latest_version()
    {
        // Arrange 
        await OnSetUpAsync(DocumentVersion.Empty());
        IDatabaseVersionService service = Provider.GetRequiredService<IDatabaseVersionService>();

        // Act
        var migrationVersion = service.GetCurrentOrLatestMigrationVersion();

        // Assert
        migrationVersion.ToString().Should().Be("0.0.3");
    }

    [Test]
    public async Task When_version_set_on_startup_Then_use_startup_version()
    {
        // Arrange 
        await OnSetUpAsync(new(0, 0, 2));
        IDatabaseVersionService service = Provider.GetRequiredService<IDatabaseVersionService>();

        // Act
        var migrationVersion = service.GetCurrentOrLatestMigrationVersion();

        // Assert
        migrationVersion.ToString().Should().Be("0.0.2");
    }
}