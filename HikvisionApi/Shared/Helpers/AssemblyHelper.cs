using System.Collections.Concurrent;
using System.Reflection;

namespace Shared.Helpers;

public class AssemblyHelper
{
    private static readonly ConcurrentDictionary<string, Assembly> _assemblies = new();

    public static Assembly? GetInstance(string key)
    {
        try
        {
            var assembly = Assembly.Load(key);

            return _assemblies.GetOrAdd(key, Assembly.Load(key));
        }
        catch (Exception)
        {
            // ignored
        }

        return null;
    }
}

public static class Assemblies
{
    public const string Domain = "Domain";
    public const string Infrastructure = "Infrastructure";
    public const string Application = "Application";
    public const string Shared = "Shared";
}