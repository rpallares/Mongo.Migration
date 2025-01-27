using System.Reflection;

namespace Mongo.Migration.Documents.Locators;

public abstract class AbstractLocator<TReturnType, TTypeIdentifier> : ILocator<TReturnType, TTypeIdentifier>
    where TReturnType : struct
    where TTypeIdentifier : class
{
    private IDictionary<TTypeIdentifier, TReturnType>? _locatesDictionary;

    protected IDictionary<TTypeIdentifier, TReturnType> LocatesDictionary
    {
        get
        {
            if (_locatesDictionary == null)
            {
                Locate();
            }

            return _locatesDictionary!;
        }

        set => _locatesDictionary = value;
    }

    public abstract TReturnType? GetLocateOrNull(TTypeIdentifier identifier);

    public abstract void Locate();

    protected IEnumerable<(Type, TAttribute)> LocateAttributes<TAttribute>() where TAttribute : Attribute
    {
        return AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetExportedTypes())
            .Select(t => (t, t.GetCustomAttributes<TAttribute>(true).FirstOrDefault()))
            .Where(tuple => tuple.Item2 is not null)
            .Cast<(Type, TAttribute)>();
    }
}