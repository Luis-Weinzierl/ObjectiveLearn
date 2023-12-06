using ObjectiveLearn.Shared;
using System.Collections.Generic;
using TankLite.Values;

namespace ObjectiveLearn.Models;

public static class VmVariables
{
    public static Dictionary<string, TankLiteValue> DefaultVariables { get; } = new()
    {
        {
            TankLiteName.Rectangle,
            new TankLiteObj
            {
                Value = new Dictionary<string, TankLiteValue>
                {
                    { TankLiteName.Constructor, new TankLiteFunc(RectangleHelpers.Constructor, TankLiteName.Rectangle) }
                }
            }
        },
        {
            TankLiteName.Triangle,
            new TankLiteObj
            {
                Value = new Dictionary<string, TankLiteValue>
                {
                    { TankLiteName.Constructor, new TankLiteFunc(TriangleHelpers.Constructor, TankLiteName.Triangle) }
                }
            }
        },
        {
            TankLiteName.Ellipse,
            new TankLiteObj
            {
                Value = new Dictionary<string, TankLiteValue>
                {
                    { TankLiteName.Constructor, new TankLiteFunc(EllipseHelpers.Constructor, TankLiteName.Ellipse) }
                }
            }
        }
    };
}
