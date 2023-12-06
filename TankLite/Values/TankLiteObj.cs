using System.Collections.Generic;

namespace TankLite.Values;

public class TankLiteObj : TankLiteValue
{
    public override bool IsReadonly { get; set; } = true;

    public override string Type { get; set; } = "object";

    public Dictionary<string, TankLiteValue> Value { get; set; }
}
