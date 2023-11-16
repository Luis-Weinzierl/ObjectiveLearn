using ObjectiveLearn.Shared;
using System.Collections.Generic;
using TankLite.Values;

namespace ObjectiveLearn.Models;

public static class VmVariables
{
    public static Dictionary<string, TlValue> DefaultVariables { get; } = new()
    {
        {
            TlName.Rectangle,
            new TlObj
            {
                Value = new Dictionary<string, TlValue>
                {
                    { TlName.Constructor, new TlFunc(RectangleHelpers.Constructor, TlName.Rectangle) }
                }
            }
        },
        {
            TlName.Triangle,
            new TlObj
            {
                Value = new Dictionary<string, TlValue>
                {
                    { TlName.Constructor, new TlFunc(TriangleHelpers.Constructor, TlName.Triangle) }
                }
            }
        },
        {
            TlName.Ellipse,
            new TlObj
            {
                Value = new Dictionary<string, TlValue>
                {
                    { TlName.Constructor, new TlFunc(EllipseHelpers.Constructor, TlName.Ellipse) }
                }
            }
        }
    };
}
