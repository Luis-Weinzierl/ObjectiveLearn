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
                { "positionSetzen", new TLFunc(SetPosition, "void") },
                { "grösseSetzen", new TLFunc(SetSize, "void") },
                { "farbeSetzen", new TLFunc(SetColor, "void") }
            }
        };

        return obj;
    }

    private static TLValue SetPosition(TLFuncArgs args)
    {
        if (args.Args.Length != 2) {
            return new TLError($"2 Argumente wurden erwartet, aber {args.Args.Length} erhalten.");
        }

        for (int i = 0; i < args.Args.Length; i++) {
            var arg = args.Args[i];

            if (arg.Type != TLName.Int) {
                return new TLError($"Das {i}. Argument sollte vom Typ int sein, ist aber {arg.Type}.");
            }
        }

        args.Parent.Value[TLName.XPos] = (TLInt)args.Args[0];
        args.Parent.Value[TLName.YPos] = (TLInt)args.Args[1];

        return new TLVoid();
    }

    private static TLValue SetSize(TLFuncArgs args)
    {
        if (args.Args.Length != 2) {
            return new TLError($"2 Argumente wurden erwartet, aber {args.Args.Length} erhalten.");
        }

        for (int i = 0; i < args.Args.Length; i++) {
            var arg = args.Args[i];

            if (arg.Type != TLName.Int) {
                return new TLError($"Das {i}. Argument sollte vom Typ int sein, ist aber {arg.Type}.");
            }
        }

        args.Parent.Value[TLName.Width] = (TLInt)args.Args[0];
        args.Parent.Value[TLName.Height] = (TLInt)args.Args[1];

        return new TLVoid();
    }

    private static TLValue SetColor(TLFuncArgs args)
    {
        if (args.Args.Length != 3 && args.Args.Length != 4) {
            return new TLError($"3 oder 4 Argumente wurden erwartet, aber {args.Args.Length} erhalten.");
        }

        for (int i = 0; i < args.Args.Length; i++) {
            var arg = args.Args[i];

            if (arg.Type != TLName.Int) {
                return new TLError($"Das {i}. Argument sollte vom Typ int sein, ist aber {arg.Type}.");
            }
            
            var value = ((TLInt)arg).Value;

            if (value > 255 || value < 0) {
                return new TLError($"Das {i}. Argument sollte zwischen 0 und 255 sein, ist aber {value}.");
            }
        }

        var color = (TLObj)args.Parent.Value[TLName.Color];

        color.Value[TLName.Red] = (TLInt)args.Args[0];
        color.Value[TLName.Green] = (TLInt)args.Args[1];
        color.Value[TLName.Blue] = (TLInt)args.Args[2];
        color.Value[TLName.Alpha] = args.Args.Length > 3 ? (TLInt)args.Args[3] : new TLInt(255);

        return new TLVoid();
    }
}
