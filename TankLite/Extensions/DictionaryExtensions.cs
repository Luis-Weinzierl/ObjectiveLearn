using System.Collections.Generic;
using System.Linq;
using TankLite.Values;

namespace TankLite.Extensions;

// ReSharper disable All

public static class DictionaryExtensions
{
    public static TankLiteValue Get(this Dictionary<string, TankLiteValue> dict, IEnumerable<string> breadcrumbs)
    {
        Dictionary<string, TankLiteValue> current = dict;

        for (int i = 0; i < breadcrumbs.Count() - 1; i++)
        {
            var name = breadcrumbs.ElementAt(i);
            if (!current.TryGetValue(name, out TankLiteValue value) || !value.Type.StartsWith("object"))
            {
                return null;
            }

            current = ((TankLiteObj)value).Value;
        }

        if (!current.TryGetValue(breadcrumbs.Last(), out TankLiteValue returnValue))
        {
            return null;
        }

        return returnValue;
    }

    public static TankLiteValue Set(this Dictionary<string, TankLiteValue> dict, IEnumerable<string> breadcrumbs, TankLiteValue tankLiteValue)
    {
        Dictionary<string, TankLiteValue> current = dict;

        for (int i = 0; i < breadcrumbs.Count() - 1; i++)
        {
            var name = breadcrumbs.ElementAt(i);

            if (!current.ContainsKey(name))
            {
                return new TankLiteError(
                    LanguageManager
                        .Get(LanguageName.TankLiteVariableDoesNotExist)
                        .Replace("{name}", string.Join('.', breadcrumbs.ToArray()[..(i + 1)]))
                );
            }

            var obj = current[name];

            if (!obj.Type.StartsWith("object"))
            {
                return new TankLiteError(
                    LanguageManager
                        .Get(LanguageName.TankLiteVariableIsNotObject)
                        .Replace("{name}", string.Join('.', breadcrumbs.ToArray()[..(i + 1)]))
                        .Replace("{type}", tankLiteValue.Type)
                );
            }

            current = ((TankLiteObj)obj).Value;
        }

        var lastName = breadcrumbs.Last();

        if (!current.ContainsKey(lastName))
        {
            return new TankLiteError(
                LanguageManager
                    .Get(LanguageName.TankLiteVariableDoesNotExist)
                    .Replace("{name}", string.Join('.', breadcrumbs))
            );
        }

        var last = current[lastName];

        if (last.IsReadonly)
        {
            return new TankLiteError(
                LanguageManager
                    .Get(LanguageName.TankLiteVariableIsReadonly)
                    .Replace("{name}", lastName)
            );
        }

        if (last.Type != tankLiteValue.Type)
        {
            return new TankLiteError(
                LanguageManager
                    .Get(LanguageName.TankLiteVarTypeInterference)
                    .Replace("{valueType}", tankLiteValue.Type)
                    .Replace("{name}", string.Join('.', breadcrumbs))
                    .Replace("{type}", last.Type)
            );
        }

        current[lastName] = tankLiteValue;

        return new TankLiteVoid();
    }
}
