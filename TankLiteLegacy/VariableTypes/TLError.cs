using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TankLite.VariableTypes;

public class TLError : ITLVariable
{
    public Type Type { get; set; } = typeof(string);

    public object Value 
    {
        get
        {
            return _message;
        }    
        set
        {
            _message = (string)value;
        }
    }

    private string _message;

    public TLError(string message)
    {
        _message = message;
    }

    public ITLVariable Add(ITLVariable other)
    {
        return new TLError("Cannot perform operation TLError");
    }

    public ITLVariable Subtract(ITLVariable other)
    {
        return new TLError("Cannot perform operation TLError");
    }

    public ITLVariable Multiply(ITLVariable other)
    {
        return new TLError("Cannot perform operation TLError");
    }

    public ITLVariable Divide(ITLVariable other)
    {
        return new TLError("Cannot perform operation TLError");
    }
}
