﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TankLite.Values;

namespace TankLite.Extensions;

public static class DictionaryExtensions
{
    public static bool Contains(this Dictionary<string, TLValue> dict, IEnumerable<string> breadcrumbs)
    {
        Dictionary<string, TLValue> current = dict;

        for (int i = 0; i < breadcrumbs.Count(); i++)
        {
            var name = breadcrumbs.ElementAt(i);
            if (!current.TryGetValue(name, out TLValue value) || !value.Type.StartsWith("object"))
            {
                return false;
            }

            current = ((TLObj)current[name]).Value;
        }

        return true;
    }

    public static TLValue Get(this Dictionary<string, TLValue> dict, IEnumerable<string> breadcrumbs)
    {
        Dictionary<string, TLValue> current = dict;

        for (int i = 0; i < breadcrumbs.Count() - 1; i++)
        {
            var name = breadcrumbs.ElementAt(i);
            if (!current.TryGetValue(name, out TLValue value) || !value.Type.StartsWith("object"))
            {
                return null;
            }

            current = ((TLObj)value).Value;
        }

        if (!current.TryGetValue(breadcrumbs.Last(), out TLValue returnValue))
        {
            return null;
        }

        return returnValue;
    }

    public static TLValue Set(this Dictionary<string, TLValue> dict, IEnumerable<string> breadcrumbs, TLValue value)
    {
        Dictionary<string, TLValue> current = dict;

        for (int i = 0; i < breadcrumbs.Count() - 1; i++)
        {
            var name = breadcrumbs.ElementAt(i);

            if (!current.ContainsKey(name))
            {
                return new TLError(
                    LanguageManager
                        .Get(LanguageName.TankLiteVariableDoesntExist)
                        .Replace("{name}", string.Join('.', breadcrumbs.ToArray()[..(i + 1)]))
                );
            }

            var obj = current[name];

            if (!obj.Type.StartsWith("object"))
            {
                return new TLError(
                    LanguageManager
                        .Get(LanguageName.TankLiteVariableIsNotObject)
                        .Replace("{name}", string.Join('.', breadcrumbs.ToArray()[..(i + 1)]))
                        .Replace("{type}", value.Type)
                );
            }

            current = ((TLObj)obj).Value;
        }

        var lastName = breadcrumbs.Last();

        if (!current.ContainsKey(lastName))
        {
            return new TLError(
                LanguageManager
                    .Get(LanguageName.TankLiteVariableDoesntExist)
                    .Replace("{name}", string.Join('.', breadcrumbs))
            );
        }

        var last = current[lastName];

        if (last.IsReadonly)
        {
            return new TLError(
                LanguageManager
                    .Get(LanguageName.TankLiteVariableIsReadonly)
                    .Replace("{name}", lastName)
            );
        }

        if (last.Type != value.Type)
        {
            return new TLError(
                LanguageManager
                    .Get(LanguageName.TankLiteVarTypeInterference)
                    .Replace("{valueType}", value.Type)
                    .Replace("{name}", string.Join('.', breadcrumbs))
                    .Replace("{type}", last.Type)
            );
        }

        current[lastName] = value;

        return new TLVoid();
    }
}
