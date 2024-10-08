using System.Reflection;
using Microsoft.Extensions.Logging;
using Mongo.Migration.Extensions;

namespace Mongo.Migration.Migrations.Locators
{
    internal class TypeMigrationDependencyLocator<TMigrationType> : MigrationLocator<TMigrationType>
        where TMigrationType : class, IMigration
    {
        private readonly IServiceProvider _serviceProvider;

        public TypeMigrationDependencyLocator(ILogger<TypeMigrationDependencyLocator<TMigrationType>> logger, IServiceProvider serviceProvider)
        : base(logger)
        {
            _serviceProvider = serviceProvider;
        }

        public override void Locate()
        {
            var migrationTypes =
                (from assembly in Assemblies
                 from type in assembly.GetTypes()
                 where typeof(TMigrationType).IsAssignableFrom(type) && !type.IsAbstract
                 select type).Distinct(new TypeComparer());

            Migrations = migrationTypes.Select(GetMigrationInstance).ToMigrationDictionary();
        }

        private TMigrationType GetMigrationInstance(Type type)
        {
            ConstructorInfo constructor = type.GetConstructors()[0];

            if (constructor != null)
            {
                object[] args = constructor
                    .GetParameters()
                    .Select(parameterInfo => _serviceProvider.GetService(parameterInfo.ParameterType))
                    .ToArray();

                return Activator.CreateInstance(type, args) as TMigrationType;
            }

            return Activator.CreateInstance(type) as TMigrationType;
        }

        private class TypeComparer : IEqualityComparer<Type>
        {
            public bool Equals(Type x, Type y)
            {
                return x.AssemblyQualifiedName == y.AssemblyQualifiedName;
            }

            public int GetHashCode(Type obj)
            {
                return obj.AssemblyQualifiedName.GetHashCode();
            }
        }
    }
}