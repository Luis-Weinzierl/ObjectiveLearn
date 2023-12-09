using System.Globalization;

namespace TankLite.Values;

public class TankLiteFloat : TankLiteValue
{
    public override string Type { get; set; } = "float";
    public float Value { get; set; }

    public TankLiteFloat(float value)
    {
        Value = value;
    }

    public override TankLiteValue Add(TankLiteValue other)
    {
        switch (other.Type)
        {
            case "float":
                Value += ((TankLiteFloat)other).Value;
                return this;

            case "int":
                Value += ((TankLiteInt)other).Value;
                return this;

            default:
                return new TankLiteError(
                    LanguageManager
                        .Get(LanguageName.TankLiteCannotAdd)
                        .Replace("{type1}", Type)
                        .Replace("{type2}", other.Type)
                );
        }
    }

    public override TankLiteValue Subtract(TankLiteValue other)
    {
        switch (other.Type)
        {
            case "float":
                Value -= ((TankLiteFloat)other).Value;
                return this;

            case "int":
                Value -= ((TankLiteInt)other).Value;
                return this;

            default:
                return new TankLiteError(
                    LanguageManager
                        .Get(LanguageName.TankLiteCannotSubtract)
                        .Replace("{type1}", Type)
                        .Replace("{type2}", other.Type)
                );
        }
    }

    public override TankLiteValue Multiply(TankLiteValue other)
    {
        switch (other.Type)
        {
            case "float":
                Value *= ((TankLiteFloat)other).Value;
                return this;

            case "int":
                Value *= ((TankLiteInt)other).Value;
                return this;

            default:
                return new TankLiteError(
                    LanguageManager
                        .Get(LanguageName.TankLiteCannotMultiply)
                        .Replace("{type1}", Type)
                        .Replace("{type2}", other.Type)
                );
        }
    }

    public override TankLiteValue Divide(TankLiteValue other)
    {
        switch (other.Type)
        {
            case "float":
                Value /= ((TankLiteFloat)other).Value;
                return this;

            case "int":
                Value /= ((TankLiteInt)other).Value;
                return this;

            default:
                return new TankLiteError(
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
