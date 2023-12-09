namespace TankLite.Values;

public abstract class TankLiteValue
{
    public virtual bool IsReadonly { get; set; } = false;
    public virtual string Type { get; set; }

    public virtual TankLiteValue Add(TankLiteValue _)
    {
        return new TankLiteError(
                    LanguageManager
                        .Get(LanguageName.TankLiteCannotOperate)
                        .Replace("{type}", Type)
                );
    }

    public virtual TankLiteValue Subtract(TankLiteValue _)
    {
        return new TankLiteError(
                    LanguageManager
                        .Get(LanguageName.TankLiteCannotOperate)
                        .Replace("{type}", Type)
                );
    }

    public virtual TankLiteValue Multiply(TankLiteValue _)
    {
        return new TankLiteError(
                    LanguageManager
                        .Get(LanguageName.TankLiteCannotOperate)
                        .Replace("{type}", Type)
                );
    }

    public virtual TankLiteValue Divide(TankLiteValue _)
    {
        return new TankLiteError(
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
