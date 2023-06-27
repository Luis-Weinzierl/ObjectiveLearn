using Eto.Drawing;
using ObjectiveLearn.Models;
using TankLite.Models;
using TankLite.Values;

namespace ObjectiveLearn.Shared;

public class ShapeHelpers
{
    public static TLObj CreateShape(TLFuncArgs args, TLString shapeType)
    {
        var x = ((TLInt)args.Args[0]).Value;
        var y = ((TLInt)args.Args[1]).Value;
        var w = ((TLInt)args.Args[2]).Value;
        var h = ((TLInt)args.Args[3]).Value;
        var a = args.Args.Length >= 5 ? ((TLInt)args.Args[4]).Value : 0;
        var r = args.Args.Length >= 8 ? ((TLInt)args.Args[5]).Value : 255;
        var g = args.Args.Length >= 8 ? ((TLInt)args.Args[6]).Value : 0;
        var b = args.Args.Length >= 8 ? ((TLInt)args.Args[7]).Value : 0;
        var alpha = args.Args.Length >= 9 ? ((TLInt)args.Args[8]).Value : 255;

        return CreateShape(
            shapeType,
            new(x, y),
            new(w, h),
            a,
            r,
            g,
            b,
            alpha
            );
    }

    public static TLObj CreateShape(
        TLString shapeType,
        Point location,
        Size size,
        int rotation,
        int r,
        int g,
        int b,
        int a
        )
    {
        var obj = new TLObj()
        {
            Value = new()
            {
                { TLName.Type, shapeType },
                { TLName.XPos, new TLInt(location.X) },
                { TLName.YPos, new TLInt(location.Y) },
                { TLName.Width, new TLInt(size.Width) },
                { TLName.Height, new TLInt(size.Height) },
                { TLName.Rotation, new TLInt(rotation) },
                {
                    TLName.Color,
                    new TLObj()
                    {
                        Value = new()
                        {
                            { TLName.Red, new TLInt(r) },
                            { TLName.Green, new TLInt(g) },
                            { TLName.Blue, new TLInt(b) },
                            { TLName.Alpha, new TLInt(a) }
                        }
                    }
                },
                { "moveX", new TLFunc(MoveX, "void") }
            }
        };

        return obj;
    }

    private static TLValue MoveX(TLFuncArgs args)
    {
        args.Parent.Value["X"] = new TLInt(300);
        return new TLVoid();
    }
}
