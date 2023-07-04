using Eto.Drawing;
using Microsoft.Extensions.Configuration;
using System.Globalization;

namespace ObjectiveLearn.Shared;

public static class ConfigurationManager
{
    private static IConfigurationRoot _config;

    public static void Init(IConfigurationRoot root)
    {
        _config = root;
    }

    public static string GetConfig(string key)
    {
        return _config[key];
    }

    public static int GetInt(string key)
    {
        return int.Parse(_config[key]);
    }

    public static float GetFloat(string key)
    {
        return float.Parse(_config[key], CultureInfo.InvariantCulture);
    }

    public static double GetDouble(string key)
    {
        return double.Parse(_config[key], CultureInfo.InvariantCulture);
    }

    public static Color GetColor(string key)
    {
        var r = int.Parse(_config[$"{key}:r"]);
        var g = int.Parse(_config[$"{key}:g"]);
        var b = int.Parse(_config[$"{key}:b"]);
        var a = int.Parse(_config[$"{key}:a"]);

        return Color.FromArgb(r, g, b, a);
    }
}
