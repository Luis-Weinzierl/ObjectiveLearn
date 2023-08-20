using System;
using System.Collections.Generic;
using System.Text;

namespace TankLite.Values;

public class TLString : TLValue
{
    public override string Type { get; set; } = "string";
    public string Value { get; set; }

    public TLString(string value)
    {
        Value = value;
    }

    public override TLValue Add(TLValue other)
    {
        switch (other.Type)
        {
            case "string":
                Value += ((TLString)other).Value;
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

    public override string ToString()
    {
        return Value;
    }
}
