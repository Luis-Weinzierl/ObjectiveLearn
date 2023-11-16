using Microsoft.Extensions.Configuration;

namespace Shared.Localization;

public static class LanguageManager
{
    private static IConfigurationRoot? _root;

    public static void Init(IConfigurationRoot root)
    {
        _root = root;
    }

    public static string Get(string name)
    {
        if (_root is not {} root || root[name] is not { } r)
        {
            throw new NullReferenceException();
        }

        return r;
    }
}