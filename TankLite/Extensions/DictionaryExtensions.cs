using System.Collections.Generic;
using System.Linq;
using TankLite.Values;

namespace TankLite.Extensions;

// ReSharper disable All

public static class DictionaryExtensions
{
    public static TlValue Get(this Dictionary<string, TlValue> dict, IEnumerable<string> breadcrumbs)
    {
        Dictionary<string, TlValue> current = dict;

        for (int i = 0; i < breadcrumbs.Count() - 1; i++)
        {
            var name = breadcrumbs.ElementAt(i);
            if (!current.TryGetValue(name, out TlValue value) || !value.Type.StartsWith("object"))
            {
                return null;
            }

            current = ((TlObj)value).Value;
        }

        if (!current.TryGetValue(breadcrumbs.Last(), out TlValue returnValue))
        {
            return null;
        }

        return returnValue;
    }

    public static TlValue Set(this Dictionary<string, TlValue> dict, IEnumerable<string> breadcrumbs, TlValue value)
    {
        Dictionary<string, TlValue> current = dict;

        for (int i = 0; i < breadcrumbs.Count() - 1; i++)
        {
            var name = breadcrumbs.ElementAt(i);

            if (!current.ContainsKey(name))
            {
                return new TlError(
                    LanguageManager
                        .Get(LanguageName.TankLiteVariableDoesNotExist)
                        .Replace("{name}", string.Join('.', breadcrumbs.ToArray()[..(i + 1)]))
                );
            }

            var obj = current[name];

            if (!obj.Type.StartsWith("object"))
            {
                return new TlError(
                    LanguageManager
                        .Get(LanguageName.TankLiteVariableIsNotObject)
                        .Replace("{name}", string.Join('.', breadcrumbs.ToArray()[..(i + 1)]))
                        .Replace("{type}", value.Type)
                );
            }

            current = ((TlObj)obj).Value;
        }

        var lastName = breadcrumbs.Last();

        if (!current.ContainsKey(lastName))
        {
            return new TlError(
                LanguageManager
                    .Get(LanguageName.TankLiteVariableDoesNotExist)
                    .Replace("{name}", string.Join('.', breadcrumbs))
            );
        }

        var last = current[lastName];

        if (last.IsReadonly)
        {
            return new TlError(
                LanguageManager
                    .Get(LanguageName.TankLiteVariableIsReadonly)
                    .Replace("{name}", lastName)
            );
        }

        if (last.Type != value.Type)
        {
            return new TlError(
                LanguageManager
                    .Get(LanguageName.TankLiteVarTypeInterference)
                    .Replace("{valueType}", value.Type)
                    .Replace("{name}", string.Join('.', breadcrumbs))
                    .Replace("{type}", last.Type)
            );
        }

        current[lastName] = value;

        return new TlVoid();
    }
}
