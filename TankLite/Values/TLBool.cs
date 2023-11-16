namespace TankLite.Values;

public class TlBool : TlValue
{
    public override string Type { get; set; } = "bool";

    public bool Value { get; set; }

    public TlBool(bool value)
    {
        Value = value;
    }

    public override string ToString()
    {
        return Value.ToString();
    }
}
