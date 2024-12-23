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
            if (uniqueHashes.Add(element.Version))
            {
                yield return element;
            }
            else
            {
                throw new DuplicateVersionException(element.GetType().Name, element.Version.ToString());
            }
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