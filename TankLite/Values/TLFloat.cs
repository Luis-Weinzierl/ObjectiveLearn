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
                return new TLError(
                    LanguageManager
                        .Get(LanguageName.TankLiteCannotAdd)
                        .Replace("{type1}", Type)
                        .Replace("{type2}", other.Type)
                );
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
                return new TLError(
                    LanguageManager
                        .Get(LanguageName.TankLiteCannotSubtract)
                        .Replace("{type1}", Type)
                        .Replace("{type2}", other.Type)
                );
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
                return new TLError(
                    LanguageManager
                        .Get(LanguageName.TankLiteCannotMultiply)
                        .Replace("{type1}", Type)
                        .Replace("{type2}", other.Type)
                );
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
                return new TLError(
                    LanguageManager
                        .Get(LanguageName.TankLiteCannotDivide)
                        .Replace("{type1}", Type)
                        .Replace("{type2}", other.Type)
                );
        }
    }

    public override string ToString()
    {
        return Value.ToString();
    }
}
