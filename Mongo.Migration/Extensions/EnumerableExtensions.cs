using Mongo.Migration.Documents;
using Mongo.Migration.Exceptions;
using Mongo.Migration.Migrations;

namespace Mongo.Migration.Extensions;

internal static class EnumerableExtensions
{
    internal static IEnumerable<TMigrationType> CheckForDuplicates<TMigrationType>(this IEnumerable<TMigrationType> list)
        where TMigrationType : class, IMigration
    {
        var uniqueHashes = new HashSet<DocumentVersion>();
        foreach (var element in list)
        {
            var version = element.Version;
            if (uniqueHashes.Add(version))
            {
                yield return element;
            }

            var typeName = element.GetType().Name;
            throw new DuplicateVersionException(typeName, element.Version.ToString());
        }
    }

    internal static IDictionary<Type, IReadOnlyCollection<TMigrationType>> ToMigrationDictionary<TMigrationType>(
        this IEnumerable<TMigrationType> migrations)
        where TMigrationType : class, IMigration
    {
        var dictionary = new Dictionary<Type, IReadOnlyCollection<TMigrationType>>();
        var list = migrations.ToList();
        var types = (from m in list select m.Type).Distinct();

        foreach (var type in types)
        {
            if (dictionary.ContainsKey(type))
            {
                continue;
            }

            var uniqueMigrations = list
                .Where(m => m.Type == type)
                .CheckForDuplicates()
                .OrderBy(m => m.Version)
                .ToList();
            dictionary.Add(type, uniqueMigrations);
        }

        return dictionary;
    }
}