using Microsoft.Extensions.Configuration;
using System.Globalization;

namespace Shared.Localisation;

public static class LanguageManager
{
    private static IConfigurationRoot _root;

    public static void Init(IConfigurationRoot root)
    {
        _root = root;
    }

    public static string Get(string name)
    {
        return _root[name];
    }
}