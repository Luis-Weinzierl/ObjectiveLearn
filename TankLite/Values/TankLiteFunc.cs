using System;
using TankLite.Models;

namespace TankLite.Values;

public class TankLiteFunc : TankLiteValue
{
    public override bool IsReadonly { get; set; } = true;
    public string ReturnType { get; set; }
    public Func<TankLiteFuncArgs, TankLiteValue> Value { get; set; }

    public TankLiteFunc(Func<TankLiteFuncArgs, TankLiteValue> func, string returnType)
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
