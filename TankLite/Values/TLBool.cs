using System;
using System.Collections.Generic;
using System.Text;

namespace TankLite.Values;

public class TLBool : TLValue
{
    public override string Type { get; set; } = "bool";

    public bool Value { get; set; }

    public TLBool(bool value)
    {
        Value = value;
    }

    public override string ToString()
    {
        return Value.ToString();
    }
}
