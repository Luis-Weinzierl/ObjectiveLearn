namespace TankLite;

public  interface ITLVariable
{
    public Type Type { get; set; }

    public object Value { get; set; }

    public abstract ITLVariable Add(ITLVariable other);
    public abstract ITLVariable Subtract(ITLVariable other);
    public abstract ITLVariable Multiply(ITLVariable other);
    public abstract ITLVariable Divide(ITLVariable other);
}
