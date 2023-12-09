namespace TankLite.Values;

public class TankLiteBool : TankLiteValue
{
    public override string Type { get; set; } = "bool";

    public bool Value { get; set; }

    public TankLiteBool(bool value)
    {
        Value = value;
    }

    public override string ToString()
    {
        return Value.ToString();
    }
}
