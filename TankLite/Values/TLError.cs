using System;

namespace TankLite.Values;

public class TlError : TlValue
{
    public static event EventHandler<string> ErrorOccurred;
    public override string Type { get; set; } = "error";
    
    public string Message { get; set; }
    public TlError(string message)
    {
        Message = message;

        ErrorOccurred?.Invoke(this, message);
    }

    public override string ToString()
    {
        return Message;
    }
}
