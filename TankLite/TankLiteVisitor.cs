global using Shared.Localization;

using Antlr4.Runtime.Misc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TankLite.Extensions;
using TankLite.Models;
using TankLite.Values;

namespace TankLite;

public class TankLiteVisitor : TankLiteBaseVisitor<TankLiteValue>
{
    public Dictionary<string, TankLiteValue> Variables { get; set; } = new();

    public override TankLiteValue VisitFncall([NotNull] TankLiteParser.FncallContext context)
    {
        var breadcrumbs = context.deepIdent().IDENT().Select(i => i.GetText()).ToArray();

        var value = Variables.Get(breadcrumbs);
        var parent = breadcrumbs.Length > 1 ? (TankLiteObj)Variables.Get(breadcrumbs[..^1]) : null;

        if (value is null) 
        {
            return new TankLiteError(
                LanguageManager
                    .Get(LanguageName.TankLiteVariableDoesNotExist)
                    .Replace("{name}", string.Join('.', breadcrumbs))
            ); 
        }

        if (!value.Type.StartsWith("func"))
        {
            return new TankLiteError(
                LanguageManager
                    .Get(LanguageName.TankLiteVariableNotFunction)
                    .Replace("{name}", string.Join('.', breadcrumbs))
                    .Replace("{type}", value.Type)
            );
        }

        var args = context.expr().Select(Visit).ToArray();
        var fn = (TankLiteFunc)value;

        var fnArgs = new TankLiteFuncArgs
        {
            Args = args,
            Parent = parent
        };

        return fn.Value(fnArgs);
    }

    public override TankLiteValue VisitConstantExpression([NotNull] TankLiteParser.ConstantExpressionContext context)
    {
        var ctx = context.constant();

        if (ctx.BOOL() is { } b)
        {
            return new TankLiteBool(b.GetText() == "true");
        }

        if (ctx.FLOAT() is { } f)
        {
            return new TankLiteFloat(float.Parse(f.GetText(), CultureInfo.InvariantCulture));
        }

        if (ctx.INT() is { } i)
        {
            return new TankLiteInt(int.Parse(i.GetText(), CultureInfo.InvariantCulture));
        }

        if (ctx.STRING() is { } s)
        {
            return new TankLiteString(s.GetText()[1..^1]);
        }

        return new TankLiteError("Shit happened");
    }

    public override TankLiteValue VisitIdentifierExpression([NotNull] TankLiteParser.IdentifierExpressionContext context)
    {
        var breadcrumbs = context.deepIdent().IDENT().Select(i => i.GetText()).ToList();

        var value = Variables.Get(breadcrumbs);

        if (value is null)
        {
            return new TankLiteError(
                LanguageManager
                    .Get(LanguageName.TankLiteVariableDoesNotExist)
                    .Replace("{name}", string.Join('.', breadcrumbs))
            );
        }

        return value;
    }

    public override TankLiteValue VisitMultiplicationExpression([NotNull] TankLiteParser.MultiplicationExpressionContext context)
    {
        var first = Visit(context.expr(0));
        var second = Visit(context.expr(1));

        return context.multOp().GetText() switch
        {
            "*" => first.Multiply(second),
            "/" => first.Divide(second),
            _ => throw new NotImplementedException()
        };
    }

    public override TankLiteValue VisitAdditiveExpression([NotNull] TankLiteParser.AdditiveExpressionContext context)
    {
        var first = Visit(context.expr(0));
        var second = Visit(context.expr(1));

        return context.addOp().GetText() switch
        {
            "+" => first.Add(second),
            "-" => first.Subtract(second),
            _ => throw new NotImplementedException()
        };
    }

    public override TankLiteValue VisitAssignment([NotNull] TankLiteParser.AssignmentContext context)
    {
        Variables[context.IDENT().GetText()] = Visit(context.expr());

        return new TankLiteVoid();
    }

    public override TankLiteValue VisitReassignment([NotNull] TankLiteParser.ReassignmentContext context)
    {
        var value = Visit(context.expr());
        var breadcrumbs = context.deepIdent().IDENT().Select(i => i.GetText()).ToList();

        Variables.Set(breadcrumbs, value);

        return new TankLiteVoid();
    }

    public override TankLiteValue VisitConstructorExpression([NotNull] TankLiteParser.ConstructorExpressionContext context)
    {
        var name = context.IDENT().GetText();

        if (!Variables.TryGetValue(name, out var value) || value.Type != "object")
        {
            return new TankLiteError(
                LanguageManager
                    .Get(LanguageName.TankLiteClassDoesNotExist)
                    .Replace("{name}", name)
            );
        }

        var constructor = ((TankLiteFunc)((TankLiteObj)value).Value[".ctor"]).Value;
        var args = context.expr().Select(Visit).ToArray();

        var fnArgs = new TankLiteFuncArgs
        {
            Args = args
        };

        return constructor(fnArgs);
    }
}
