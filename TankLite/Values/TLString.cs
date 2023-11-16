namespace TankLite.Values;

public class TlString : TlValue
{
    public override string Type { get; set; } = "string";
    public string Value { get; set; }

    public TlString(string value)
    {
        Value = value;
    }

    public override TlValue Add(TlValue other)
    {
        switch (other.Type)
        {
            case "string":
                Value += ((TlString)other).Value;
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

    public override string ToString()
    {
        return Value;
    }
}
