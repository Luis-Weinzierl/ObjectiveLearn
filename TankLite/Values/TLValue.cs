namespace TankLite.Values;

public abstract class TlValue
{
    public virtual bool IsReadonly { get; set; } = false;
    public virtual string Type { get; set; }

    public virtual TlValue Add(TlValue _)
    {
        return new TlError(
                    LanguageManager
                        .Get(LanguageName.TankLiteCannotOperate)
                        .Replace("{type}", Type)
                );
    }

    public virtual TlValue Subtract(TlValue _)
    {
        return new TlError(
                    LanguageManager
                        .Get(LanguageName.TankLiteCannotOperate)
                        .Replace("{type}", Type)
                );
    }

    public virtual TlValue Multiply(TlValue _)
    {
        return new TlError(
                    LanguageManager
                        .Get(LanguageName.TankLiteCannotOperate)
                        .Replace("{type}", Type)
                );
    }

    public virtual TlValue Divide(TlValue _)
    {
        return new TlError(
                    LanguageManager
                        .Get(LanguageName.TankLiteCannotOperate)
                        .Replace("{type}", Type)
                );
    }

    public override string ToString()
    {
        return Type;
    }
}
