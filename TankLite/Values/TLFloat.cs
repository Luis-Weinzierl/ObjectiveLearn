using System.Globalization;

namespace TankLite.Values;

public class TlFloat : TlValue
{
    public override string Type { get; set; } = "float";
    public float Value { get; set; }

    public TlFloat(float value)
    {
        Value = value;
    }

    public override TlValue Add(TlValue other)
    {
        switch (other.Type)
        {
            case "float":
                Value += ((TlFloat)other).Value;
                return this;

            case "int":
                Value += ((TlInt)other).Value;
                return this;

            default:
                return new TlError(
                    LanguageManager
                        .Get(LanguageName.TankLiteCannotAdd)
                        .Replace("{type1}", Type)
                        .Replace("{type2}", other.Type)
                );
        }
    }

    public override TlValue Subtract(TlValue other)
    {
        switch (other.Type)
        {
            case "float":
                Value -= ((TlFloat)other).Value;
                return this;

            case "int":
                Value -= ((TlInt)other).Value;
                return this;

            default:
                return new TlError(
                    LanguageManager
                        .Get(LanguageName.TankLiteCannotSubtract)
                        .Replace("{type1}", Type)
                        .Replace("{type2}", other.Type)
                );
        }
    }

    public override TlValue Multiply(TlValue other)
    {
        switch (other.Type)
        {
            case "float":
                Value *= ((TlFloat)other).Value;
                return this;

            case "int":
                Value *= ((TlInt)other).Value;
                return this;

            default:
                return new TlError(
                    LanguageManager
                        .Get(LanguageName.TankLiteCannotMultiply)
                        .Replace("{type1}", Type)
                        .Replace("{type2}", other.Type)
                );
        }
    }

    public override TlValue Divide(TlValue other)
    {
        switch (other.Type)
        {
            case "float":
                Value /= ((TlFloat)other).Value;
                return this;

            case "int":
                Value /= ((TlInt)other).Value;
                return this;

            default:
                return new TlError(
                    LanguageManager
                        .Get(LanguageName.TankLiteCannotDivide)
                        .Replace("{type1}", Type)
                        .Replace("{type2}", other.Type)
                );
        }
    }

    public override string ToString()
    {
        return Value.ToString(CultureInfo.CurrentCulture);
    }
}
