using Eto.Drawing;
using Microsoft.Extensions.Configuration;
using System.Globalization;

namespace ObjectiveLearn.Shared;

public static class ConfigManager
{
    private static IConfigurationRoot _config;

    public static void Init(IConfigurationRoot root)
    {
        _config = root;
    }

    public static string Get(string key)
    {
        return _config[key];
    }

    public static int GetInt(string key)
    {
        return int.Parse(Get(key));
    }

    public static float GetFloat(string key)
    {
        return float.Parse(Get(key), CultureInfo.InvariantCulture);
    }

    public static double GetDouble(string key)
    {
        return double.Parse(Get(key), CultureInfo.InvariantCulture);
    }

    public static Color GetColor(string key)
    {
        var r = GetInt($"{key}:r");
        var g = GetInt($"{key}:g");
        var b = GetInt($"{key}:b");
        var a = GetInt($"{key}:a");

        return Color.FromArgb(r, g, b, a);
    }
}
