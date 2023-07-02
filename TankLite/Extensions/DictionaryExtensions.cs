using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
}
