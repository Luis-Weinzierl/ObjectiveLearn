using ObjectiveLearn.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TankLite.Values;

namespace ObjectiveLearn.Models;

public static class VMVariables
{
    public static readonly Dictionary<string, TLValue> DefaultVariables = new()
    {
        {
            TLName.Rectangle,
            new TLObj()
            {
                Value = new()
                {
                    { TLName.Constructor, new TLFunc(RectangleHelpers.Constructor, TLName.Rectangle) }
                }
            }
        },
        {
            TLName.Triangle,
            new TLObj()
            {
                Value = new()
                {
                    { TLName.Constructor, new TLFunc(TriangleHelpers.Constructor, TLName.Triangle) }
                }
            }
        },
        {
            TLName.Ellipse,
            new TLObj()
            {
                Value = new()
                {
                    { TLName.Constructor, new TLFunc(EllipseHelpers.Constructor, TLName.Ellipse) }
                }
            }
        }
    };
}
