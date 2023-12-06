namespace TankLite.Values;

public class TankLiteString : TankLiteValue
{
    public override string Type { get; set; } = "string";
    public string Value { get; set; }

    public TankLiteString(string value)
    {
        Value = value;
    }

    public override TankLiteValue Add(TankLiteValue other)
    {
        switch (other.Type)
        {
            case "string":
                Value += ((TankLiteString)other).Value;
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

    public override string ToString()
    {
        return Value;
    }
}
