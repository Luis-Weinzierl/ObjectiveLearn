using Eto.Drawing;
using ObjectiveLearn.Models;
using TankLite.Models;
using TankLite.Values;
using Shared.Localisation;

namespace ObjectiveLearn.Shared;

public class ShapeHelpers
{
    public static TLObj CreateShape(SerializeableShape shape)
    {
        return CreateShape(
            new(shape.Type),
            new(shape.X, shape.Y),
            new(shape.Width, shape.Height),
            shape.Rotation,
            shape.R,
            shape.G,
            shape.B,
            shape.A
            );
    }

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
                { TLName.SetPosition, new TLFunc(SetPosition, "void") },
                { TLName.SetSize, new TLFunc(SetSize, "void") },
                { TLName.SetColor, new TLFunc(SetColor, "void") },
                { TLName.SetRotation, new TLFunc(SetRotation, "void") },
                { TLName.Move, new TLFunc(Move, "void") },
            }
        };
        return obj;
    }

    private static TLValue SetPosition(TLFuncArgs args)
    {
        if (args.Args.Length != 2) {
            return new TLError(
                LanguageManager
                    .Get(LanguageName.ErrorExpectedArgs)
                    .Replace("{expected}", "2")
                    .Replace("{received}", args.Args.Length.ToString())
            );
        }

        for (int i = 0; i < args.Args.Length; i++) {
            var arg = args.Args[i];

            if (arg.Type != TLName.Int) {
                return new TLError(
                    LanguageManager
                        .Get(LanguageName.ErrorTypeInferrenceArgs)
                        .Replace("{i}", i.ToString())
                        .Replace("{type}", arg.Type)
                );
            }
        }

        args.Parent.Value[TLName.XPos] = (TLInt)args.Args[0];
        args.Parent.Value[TLName.YPos] = (TLInt)args.Args[1];

        return new TLVoid();
    }

    private static TLValue SetSize(TLFuncArgs args)
    {
        if (args.Args.Length != 2) {
            return new TLError(
                LanguageManager
                    .Get(LanguageName.ErrorExpectedArgs)
                    .Replace("{expected}", "2")
                    .Replace("{received}", args.Args.Length.ToString())
            );
        }

        for (int i = 0; i < args.Args.Length; i++) {
            var arg = args.Args[i];

            if (arg.Type != TLName.Int) {
                return new TLError(
                    LanguageManager
                        .Get(LanguageName.ErrorTypeInferrenceArgs)
                        .Replace("{i}", i.ToString())
                        .Replace("{type}", arg.Type)
                );
            }
        }

        args.Parent.Value[TLName.Width] = (TLInt)args.Args[0];
        args.Parent.Value[TLName.Height] = (TLInt)args.Args[1];

        return new TLVoid();
    }

    private static TLValue SetColor(TLFuncArgs args)
    {
        if (args.Args.Length != 3 && args.Args.Length != 4) {
            return new TLError(
                LanguageManager
                    .Get(LanguageName.ErrorExpectedArgs)
                    .Replace("{expected}", "3 {or} 4")
                    .Replace("{or}", LanguageManager.Get(LanguageName.ErrorOr))
                    .Replace("{received}", args.Args.Length.ToString())
            );
        }

        for (int i = 0; i < args.Args.Length; i++) {
            var arg = args.Args[i];

            if (arg.Type != TLName.Int) {
                return new TLError(
                    LanguageManager
                        .Get(LanguageName.ErrorTypeInferrenceArgs)
                        .Replace("{i}", i.ToString())
                        .Replace("{type}", arg.Type)
                );
            }
            
            var value = ((TLInt)arg).Value;

            if (value > 255 || value < 0) {
                return new TLError(
                    LanguageManager
                        .Get(LanguageName.ErrorOutsideRange)
                        .Replace("{i}", i.ToString())
                        .Replace("{min}", "0")
                        .Replace("{max}", "255")
                        .Replace("{value}", value.ToString())
                );
            }
        }

        var color = (TLObj)args.Parent.Value[TLName.Color];

        color.Value[TLName.Red] = (TLInt)args.Args[0];
        color.Value[TLName.Green] = (TLInt)args.Args[1];
        color.Value[TLName.Blue] = (TLInt)args.Args[2];
        color.Value[TLName.Alpha] = args.Args.Length > 3 ? (TLInt)args.Args[3] : new TLInt(255);

        return new TLVoid();
    }

    private static TLValue SetRotation(TLFuncArgs args)
    {
        if (args.Args.Length != 1) {
            return new TLError(
                LanguageManager
                    .Get(LanguageName.ErrorExpectedArgs)
                    .Replace("{expected}", "1")
                    .Replace("{received}", args.Args.Length.ToString())
            );
        }

        if (args.Args[0].Type != TLName.Int) {
            return new TLError(
                    LanguageManager
                        .Get(LanguageName.ErrorTypeInferrenceArgs)
                        .Replace("{i}", "0")
                        .Replace("{type}", args.Args[0].Type)
                );
        }

        args.Parent.Value[TLName.Rotation] = (TLInt)args.Args[0];

        return new TLVoid();
    }

    private static TLValue Move(TLFuncArgs args)
    {
        if (args.Args.Length != 2)
        {
            return new TLError(
                LanguageManager
                    .Get(LanguageName.ErrorExpectedArgs)
                    .Replace("{expected}", "2")
                    .Replace("{received}", args.Args.Length.ToString())
            );
        }

        foreach (var arg in args.Args)
        {
            if (arg.Type != TLName.Int)
            {
                return new TLError(
                    LanguageManager
                        .Get(LanguageName.ErrorTypeInferrenceArgs)
                        .Replace("{i}", "0")
                        .Replace("{type}", args.Args[0].Type)
                );
            }
        }

        args.Parent.Value[TLName.XPos].Add(args.Args[0]);
        args.Parent.Value[TLName.YPos].Add(args.Args[1]);

        return new TLVoid();
    }
}
