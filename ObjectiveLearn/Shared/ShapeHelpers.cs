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
    public static TankLiteObj CreateShape(SerializableShape shape)
    {
        return CreateShape(
            new TankLiteString(shape.Type),
            new Point(shape.X, shape.Y),
            new Size(shape.Width, shape.Height),
            shape.Rotation,
            shape.R,
            shape.G,
            shape.B,
            shape.A
            );
    }

    public static TankLiteObj CreateShape(TankLiteFuncArgs args, TankLiteString shapeType)
    {
        var x = ((TankLiteInt)args.Args[0]).Value;
        var y = ((TankLiteInt)args.Args[1]).Value;
        var w = ((TankLiteInt)args.Args[2]).Value;
        var h = ((TankLiteInt)args.Args[3]).Value;
        var a = args.Args.Length >= 5 ? ((TankLiteInt)args.Args[4]).Value : 0;
        var r = args.Args.Length >= 8 ? ((TankLiteInt)args.Args[5]).Value : 255;
        var g = args.Args.Length >= 8 ? ((TankLiteInt)args.Args[6]).Value : 0;
        var b = args.Args.Length >= 8 ? ((TankLiteInt)args.Args[7]).Value : 0;
        var alpha = args.Args.Length >= 9 ? ((TankLiteInt)args.Args[8]).Value : 255;

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

    public static TankLiteObj CreateShape(
        TankLiteString shapeType,
        Point location,
        Size size,
        int rotation,
        int r,
        int g,
        int b,
        int a
        )
    {
        var obj = new TankLiteObj
        {
            Value = new Dictionary<string, TankLiteValue>
            {
                { TankLiteName.Type, shapeType },
                { TankLiteName.XPos, new TankLiteInt(location.X) },
                { TankLiteName.YPos, new TankLiteInt(location.Y) },
                { TankLiteName.Width, new TankLiteInt(size.Width) },
                { TankLiteName.Height, new TankLiteInt(size.Height) },
                { TankLiteName.Rotation, new TankLiteInt(rotation) },
                {
                    TankLiteName.Color,
                    new TankLiteObj
                    {
                        Value = new Dictionary<string, TankLiteValue>
                        {
                            { TankLiteName.Red, new TankLiteInt(r) },
                            { TankLiteName.Green, new TankLiteInt(g) },
                            { TankLiteName.Blue, new TankLiteInt(b) },
                            { TankLiteName.Alpha, new TankLiteInt(a) }
                        }
                    }
                },
                { TankLiteName.SetPosition, new TankLiteFunc(SetPosition, "void") },
                { TankLiteName.SetSize, new TankLiteFunc(SetSize, "void") },
                { TankLiteName.SetColor, new TankLiteFunc(SetColor, "void") },
                { TankLiteName.SetRotation, new TankLiteFunc(SetRotation, "void") },
                { TankLiteName.Move, new TankLiteFunc(Move, "void") }
            }
        };
        return obj;
    }

    private static TankLiteValue SetPosition(TankLiteFuncArgs args)
    {
        if (args.Args.Length != 2) {
            return new TankLiteError(
                LanguageManager
                    .Get(LanguageName.ErrorExpectedArgs)
                    .Replace("{expected}", "2")
                    .Replace("{received}", args.Args.Length.ToString())
            );
        }

        for (var i = 0; i < args.Args.Length; i++) {
            var arg = args.Args[i];

            if (arg.Type != TankLiteName.Int) {
                return new TankLiteError(
                    LanguageManager
                        .Get(LanguageName.ErrorTypeInferenceArgs)
                        .Replace("{i}", i.ToString())
                        .Replace("{type}", arg.Type)
                );
            }
        }

        args.Parent.Value[TankLiteName.XPos] = (TankLiteInt)args.Args[0];
        args.Parent.Value[TankLiteName.YPos] = (TankLiteInt)args.Args[1];

        return new TankLiteVoid();
    }

    private static TankLiteValue SetSize(TankLiteFuncArgs args)
    {
        if (args.Args.Length != 2) {
            return new TankLiteError(
                LanguageManager
                    .Get(LanguageName.ErrorExpectedArgs)
                    .Replace("{expected}", "2")
                    .Replace("{received}", args.Args.Length.ToString())
            );
        }

        for (var i = 0; i < args.Args.Length; i++) {
            var arg = args.Args[i];

            if (arg.Type != TankLiteName.Int) {
                return new TankLiteError(
                    LanguageManager
                        .Get(LanguageName.ErrorTypeInferenceArgs)
                        .Replace("{i}", i.ToString())
                        .Replace("{type}", arg.Type)
                );
            }
        }

        args.Parent.Value[TankLiteName.Width] = (TankLiteInt)args.Args[0];
        args.Parent.Value[TankLiteName.Height] = (TankLiteInt)args.Args[1];

        return new TankLiteVoid();
    }

    private static TankLiteValue SetColor(TankLiteFuncArgs args)
    {
        if (args.Args.Length != 3 && args.Args.Length != 4) {
            return new TankLiteError(
                LanguageManager
                    .Get(LanguageName.ErrorExpectedArgs)
                    .Replace("{expected}", "3 {or} 4")
                    .Replace("{or}", LanguageManager.Get(LanguageName.ErrorOr))
                    .Replace("{received}", args.Args.Length.ToString())
            );
        }

        for (var i = 0; i < args.Args.Length; i++) {
            var arg = args.Args[i];

            if (arg.Type != TankLiteName.Int) {
                return new TankLiteError(
                    LanguageManager
                        .Get(LanguageName.ErrorTypeInferenceArgs)
                        .Replace("{i}", i.ToString())
                        .Replace("{type}", arg.Type)
                );
            }
            
            var value = ((TankLiteInt)arg).Value;

            if (value is > 255 or < 0) {
                return new TankLiteError(
                    LanguageManager
                        .Get(LanguageName.ErrorOutsideRange)
                        .Replace("{i}", i.ToString())
                        .Replace("{min}", "0")
                        .Replace("{max}", "255")
                        .Replace("{value}", value.ToString())
                );
            }
        }

        var color = (TankLiteObj)args.Parent.Value[TankLiteName.Color];

        color.Value[TankLiteName.Red] = (TankLiteInt)args.Args[0];
        color.Value[TankLiteName.Green] = (TankLiteInt)args.Args[1];
        color.Value[TankLiteName.Blue] = (TankLiteInt)args.Args[2];
        color.Value[TankLiteName.Alpha] = args.Args.Length > 3 ? (TankLiteInt)args.Args[3] : new TankLiteInt(255);

        return new TankLiteVoid();
    }

    private static TankLiteValue SetRotation(TankLiteFuncArgs args)
    {
        if (args.Args.Length != 1) {
            return new TankLiteError(
                LanguageManager
                    .Get(LanguageName.ErrorExpectedArgs)
                    .Replace("{expected}", "1")
                    .Replace("{received}", args.Args.Length.ToString())
            );
        }

        if (args.Args[0].Type != TankLiteName.Int) {
            return new TankLiteError(
                    LanguageManager
                        .Get(LanguageName.ErrorTypeInferenceArgs)
                        .Replace("{i}", "0")
                        .Replace("{type}", args.Args[0].Type)
                );
        }

        args.Parent.Value[TankLiteName.Rotation] = (TankLiteInt)args.Args[0];

        return new TankLiteVoid();
    }

    private static TankLiteValue Move(TankLiteFuncArgs args)
    {
        if (args.Args.Length != 2)
        {
            return new TankLiteError(
                LanguageManager
                    .Get(LanguageName.ErrorExpectedArgs)
                    .Replace("{expected}", "2")
                    .Replace("{received}", args.Args.Length.ToString())
            );
        }

        var anyIsInt = args.Args
            .Any(arg => arg.Type != TankLiteName.Int);

        if (anyIsInt)
        {
            return new TankLiteError(
                LanguageManager
                    .Get(LanguageName.ErrorTypeInferenceArgs)
                    .Replace("{i}", "0")
                    .Replace("{type}", args.Args[0].Type)
            );
        }

        args.Parent.Value[TankLiteName.XPos].Add(args.Args[0]);
        args.Parent.Value[TankLiteName.YPos].Add(args.Args[1]);

        return new TankLiteVoid();
    }
}
