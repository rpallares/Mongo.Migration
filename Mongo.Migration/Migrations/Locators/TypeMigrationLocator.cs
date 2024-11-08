using Microsoft.Extensions.Logging;
using Mongo.Migration.Extensions;
using Mongo.Migration.Migrations.Document;

namespace Mongo.Migration.Migrations.Locators
{
    internal class TypeMigrationLocator : MigrationLocator<IDocumentMigration>
    {
        public TypeMigrationLocator(ILogger<TypeMigrationLocator> logger) : base(logger) { }

        public override void Locate()
        {
            var migrationTypes =
                (from assembly in Assemblies
                 from type in assembly.GetTypes()
                 where typeof(IDocumentMigration).IsAssignableFrom(type) && !type.IsAbstract
                 select type).Distinct();

            Migrations = migrationTypes
                .Select(t =>
                    Activator.CreateInstance(t) as IDocumentMigration
                    ?? throw new InvalidOperationException($"Cannot create {t} document migration"))
                .ToMigrationDictionary();
        }
    }
}