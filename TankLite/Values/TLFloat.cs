using System;
using System.Collections.Generic;
using System.Text;

namespace TankLite.Values;

public class TLFloat : TLValue
{
    public override string Type { get; set; } = "float";
    public float Value { get; set; }

    public TLFloat(float value)
    {
        Value = value;
    }

    public override TLValue Add(TLValue other)
    {
        switch (other.Type)
        {
            case "float":
                Value += ((TLFloat)other).Value;
                return this;

            case "int":
                Value += ((TLInt)other).Value;
                return this;

            default:
                return new TLError($"Cannot add {Type} and {other.Type}");
        }
    }

    public override TLValue Subtract(TLValue other)
    {
        switch (other.Type)
        {
            case "float":
                Value -= ((TLFloat)other).Value;
                return this;

            case "int":
                Value -= ((TLInt)other).Value;
                return this;

            default:
                return new TLError($"Cannot subtract {other.Type} from {Type}");
        }
    }

    public override TLValue Multiply(TLValue other)
    {
        switch (other.Type)
        {
            case "float":
                Value *= ((TLFloat)other).Value;
                return this;

            case "int":
                Value *= ((TLInt)other).Value;
                return this;

            default:
                return new TLError($"Cannot multiply {Type} by {other.Type}");
        }
    }

    public override TLValue Divide(TLValue other)
    {
        switch (other.Type)
        {
            case "float":
                Value /= ((TLFloat)other).Value;
                return this;

            case "int":
                Value /= ((TLInt)other).Value;
                return this;

            default:
                return new TLError($"Cannot divide {Type} by {other.Type}");
        }
    }

    public override string ToString()
    {
        return Value.ToString();
    }
}
