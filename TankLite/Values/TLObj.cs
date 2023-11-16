using System.Collections.Generic;

namespace TankLite.Values;

public class TlObj : TlValue
{
    public override bool IsReadonly { get; set; } = true;

    public override string Type { get; set; } = "object";

    public Dictionary<string, TlValue> Value { get; set; }
}
