using System.Collections.Generic;
using System.Linq;
using Eto.Drawing;
using ObjectiveLearn.Models;
using TankLite.Models;
using TankLite.Values;
using Shared.Localization;

namespace ObjectiveLearn.Shared;

public class ShapeHelpers
{
    public static TlObj CreateShape(SerializableShape shape)
    {
        return CreateShape(
            new TlString(shape.Type),
            new Point(shape.X, shape.Y),
            new Size(shape.Width, shape.Height),
            shape.Rotation,
            shape.R,
            shape.G,
            shape.B,
            shape.A
            );
    }

    public static TlObj CreateShape(TlFuncArgs args, TlString shapeType)
    {
        var x = ((TlInt)args.Args[0]).Value;
        var y = ((TlInt)args.Args[1]).Value;
        var w = ((TlInt)args.Args[2]).Value;
        var h = ((TlInt)args.Args[3]).Value;
        var a = args.Args.Length >= 5 ? ((TlInt)args.Args[4]).Value : 0;
        var r = args.Args.Length >= 8 ? ((TlInt)args.Args[5]).Value : 255;
        var g = args.Args.Length >= 8 ? ((TlInt)args.Args[6]).Value : 0;
        var b = args.Args.Length >= 8 ? ((TlInt)args.Args[7]).Value : 0;
        var alpha = args.Args.Length >= 9 ? ((TlInt)args.Args[8]).Value : 255;

        return CreateShape(
            shapeType,
            new Point(x, y),
            new Size(w, h),
            a,
            r,
            g,
            b,
            alpha
            );
    }

    public static TlObj CreateShape(
        TlString shapeType,
        Point location,
        Size size,
        int rotation,
        int r,
        int g,
        int b,
        int a
        )
    {
        var obj = new TlObj
        {
            Value = new Dictionary<string, TlValue>
            {
                { TlName.Type, shapeType },
                { TlName.XPos, new TlInt(location.X) },
                { TlName.YPos, new TlInt(location.Y) },
                { TlName.Width, new TlInt(size.Width) },
                { TlName.Height, new TlInt(size.Height) },
                { TlName.Rotation, new TlInt(rotation) },
                {
                    TlName.Color,
                    new TlObj
                    {
                        Value = new Dictionary<string, TlValue>
                        {
                            { TlName.Red, new TlInt(r) },
                            { TlName.Green, new TlInt(g) },
                            { TlName.Blue, new TlInt(b) },
                            { TlName.Alpha, new TlInt(a) }
                        }
                    }
                },
                { TlName.SetPosition, new TlFunc(SetPosition, "void") },
                { TlName.SetSize, new TlFunc(SetSize, "void") },
                { TlName.SetColor, new TlFunc(SetColor, "void") },
                { TlName.SetRotation, new TlFunc(SetRotation, "void") },
                { TlName.Move, new TlFunc(Move, "void") }
            }
        };
        return obj;
    }

    private static TlValue SetPosition(TlFuncArgs args)
    {
        if (args.Args.Length != 2) {
            return new TlError(
                LanguageManager
                    .Get(LanguageName.ErrorExpectedArgs)
                    .Replace("{expected}", "2")
                    .Replace("{received}", args.Args.Length.ToString())
            );
        }

        for (var i = 0; i < args.Args.Length; i++) {
            var arg = args.Args[i];

            if (arg.Type != TlName.Int) {
                return new TlError(
                    LanguageManager
                        .Get(LanguageName.ErrorTypeInferenceArgs)
                        .Replace("{i}", i.ToString())
                        .Replace("{type}", arg.Type)
                );
            }
        }

        args.Parent.Value[TlName.XPos] = (TlInt)args.Args[0];
        args.Parent.Value[TlName.YPos] = (TlInt)args.Args[1];

        return new TlVoid();
    }

    private static TlValue SetSize(TlFuncArgs args)
    {
        if (args.Args.Length != 2) {
            return new TlError(
                LanguageManager
                    .Get(LanguageName.ErrorExpectedArgs)
                    .Replace("{expected}", "2")
                    .Replace("{received}", args.Args.Length.ToString())
            );
        }

        for (var i = 0; i < args.Args.Length; i++) {
            var arg = args.Args[i];

            if (arg.Type != TlName.Int) {
                return new TlError(
                    LanguageManager
                        .Get(LanguageName.ErrorTypeInferenceArgs)
                        .Replace("{i}", i.ToString())
                        .Replace("{type}", arg.Type)
                );
            }
        }

        args.Parent.Value[TlName.Width] = (TlInt)args.Args[0];
        args.Parent.Value[TlName.Height] = (TlInt)args.Args[1];

        return new TlVoid();
    }

    private static TlValue SetColor(TlFuncArgs args)
    {
        if (args.Args.Length != 3 && args.Args.Length != 4) {
            return new TlError(
                LanguageManager
                    .Get(LanguageName.ErrorExpectedArgs)
                    .Replace("{expected}", "3 {or} 4")
                    .Replace("{or}", LanguageManager.Get(LanguageName.ErrorOr))
                    .Replace("{received}", args.Args.Length.ToString())
            );
        }

        for (var i = 0; i < args.Args.Length; i++) {
            var arg = args.Args[i];

            if (arg.Type != TlName.Int) {
                return new TlError(
                    LanguageManager
                        .Get(LanguageName.ErrorTypeInferenceArgs)
                        .Replace("{i}", i.ToString())
                        .Replace("{type}", arg.Type)
                );
            }
            
            var value = ((TlInt)arg).Value;

            if (value is > 255 or < 0) {
                return new TlError(
                    LanguageManager
                        .Get(LanguageName.ErrorOutsideRange)
                        .Replace("{i}", i.ToString())
                        .Replace("{min}", "0")
                        .Replace("{max}", "255")
                        .Replace("{value}", value.ToString())
                );
            }
        }

        var color = (TlObj)args.Parent.Value[TlName.Color];

        color.Value[TlName.Red] = (TlInt)args.Args[0];
        color.Value[TlName.Green] = (TlInt)args.Args[1];
        color.Value[TlName.Blue] = (TlInt)args.Args[2];
        color.Value[TlName.Alpha] = args.Args.Length > 3 ? (TlInt)args.Args[3] : new TlInt(255);

        return new TlVoid();
    }

    private static TlValue SetRotation(TlFuncArgs args)
    {
        if (args.Args.Length != 1) {
            return new TlError(
                LanguageManager
                    .Get(LanguageName.ErrorExpectedArgs)
                    .Replace("{expected}", "1")
                    .Replace("{received}", args.Args.Length.ToString())
            );
        }

        if (args.Args[0].Type != TlName.Int) {
            return new TlError(
                    LanguageManager
                        .Get(LanguageName.ErrorTypeInferenceArgs)
                        .Replace("{i}", "0")
                        .Replace("{type}", args.Args[0].Type)
                );
        }

        args.Parent.Value[TlName.Rotation] = (TlInt)args.Args[0];

        return new TlVoid();
    }

    private static TlValue Move(TlFuncArgs args)
    {
        if (args.Args.Length != 2)
        {
            return new TlError(
                LanguageManager
                    .Get(LanguageName.ErrorExpectedArgs)
                    .Replace("{expected}", "2")
                    .Replace("{received}", args.Args.Length.ToString())
            );
        }

        var anyIsInt = args.Args
            .Any(arg => arg.Type != TlName.Int);

        if (anyIsInt)
        {
            return new TlError(
                LanguageManager
                    .Get(LanguageName.ErrorTypeInferenceArgs)
                    .Replace("{i}", "0")
                    .Replace("{type}", args.Args[0].Type)
            );
        }

        args.Parent.Value[TlName.XPos].Add(args.Args[0]);
        args.Parent.Value[TlName.YPos].Add(args.Args[1]);

        return new TlVoid();
    }
}
