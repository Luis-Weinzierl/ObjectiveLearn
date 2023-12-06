namespace TankLite.Values;

public class TankLiteInt : TankLiteValue
{
    public override string Type { get; set; } = "int";
    public int Value { get; set; }

    public TankLiteInt(int value)
    {
        Value = value;
    }

    public override TankLiteValue Add(TankLiteValue other)
    {
        switch (other.Type)
        {
            case "float":
                ((TankLiteFloat)other).Value += Value;
                return other;

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
                var flt = new TankLiteFloat(Value);
                flt.Value -= ((TankLiteFloat)other).Value;
                return flt;

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
                ((TankLiteFloat)other).Value *= Value;
                return other;

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
                var flt = new TankLiteFloat(Value);
                flt.Value /= ((TankLiteFloat)other).Value;
                return flt;

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
        return Value.ToString();
    }
}
