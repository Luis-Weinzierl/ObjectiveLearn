using System;
using TankLite.Models;

namespace TankLite.Values;

public class TlFunc : TlValue
{
    public override bool IsReadonly { get; set; } = true;
    public string ReturnType { get; set; }
    public Func<TlFuncArgs, TlValue> Value { get; set; }

    public TlFunc(Func<TlFuncArgs, TlValue> func, string returnType)
    {
        Value = func;
        base.Type = $"func<{returnType}>";
        ReturnType = returnType;
    }

    public override string ToString()
    {
        return $"() => {ReturnType}";
    }
}
