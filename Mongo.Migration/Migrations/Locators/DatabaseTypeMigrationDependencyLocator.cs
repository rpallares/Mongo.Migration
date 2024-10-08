using Microsoft.Extensions.Logging;
using Mongo.Migration.Migrations.Database;

namespace Mongo.Migration.Migrations.Locators
{
    internal class DatabaseTypeMigrationDependencyLocator : TypeMigrationDependencyLocator<IDatabaseMigration>, IDatabaseTypeMigrationDependencyLocator
    {
        private IDictionary<Type, IReadOnlyCollection<IDatabaseMigration>> _migrations;

        protected override IDictionary<Type, IReadOnlyCollection<IDatabaseMigration>> Migrations
        {
            get
            {
                if (_migrations == null)
                {
                    Locate();
                }

                return _migrations;
            }
            set
            {
                _migrations = value;
            }
        }

        public DatabaseTypeMigrationDependencyLocator(ILogger<DatabaseTypeMigrationDependencyLocator> logger, IServiceProvider serviceProvider)
            : base(logger, serviceProvider)
        {
        }
    }
}