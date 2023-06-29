using System;
using System.Collections.Generic;
using System.Text;

namespace TankLite.Values;

public class TLError : TLValue
{
    public static event EventHandler<string> ErrorOccurred;
    public override string Type { get; set; } = "error";
    
    public string Message { get; set; }
    public TLError(string message)
    {
        Message = message;
        ErrorOccurred.Invoke(this, message);
    }

    public override string ToString()
    {
        return Message;
    }
}
