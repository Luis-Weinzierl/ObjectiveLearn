namespace TankLite.Values;

public class TlInt : TlValue
{
    public override string Type { get; set; } = "int";
    public int Value { get; set; }

    public TlInt(int value)
    {
        Value = value;
    }

    public override TlValue Add(TlValue other)
    {
        switch (other.Type)
        {
            case "float":
                ((TlFloat)other).Value += Value;
                return other;

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
                var flt = new TlFloat(Value);
                flt.Value -= ((TlFloat)other).Value;
                return flt;

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
                ((TlFloat)other).Value *= Value;
                return other;

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
                var flt = new TlFloat(Value);
                flt.Value /= ((TlFloat)other).Value;
                return flt;

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
        return Value.ToString();
    }
}
