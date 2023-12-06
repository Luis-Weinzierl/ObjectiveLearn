using System;

namespace TankLite.Values;

public class TankLiteError : TankLiteValue
{
    public static event EventHandler<string> ErrorOccurred;
    public override string Type { get; set; } = "error";
    
    public string Message { get; set; }
    public TankLiteError(string message)
    {
        Message = message;

        ErrorOccurred?.Invoke(this, message);
    }

    public override string ToString()
    {
        return Message;
    }
}
