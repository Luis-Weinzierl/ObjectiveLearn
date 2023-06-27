using System;
using System.Collections.Generic;
using System.Text;

namespace TankLite.Values;

public class TLError : TLValue
{
    public override string Type { get; set; } = "error";
    
    public string Message { get; set; }
    public TLError(string message)
    {
        Message = message;
    }

    public override string ToString()
    {
        return Message;
    }
}
