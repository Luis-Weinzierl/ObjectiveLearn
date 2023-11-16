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

public class TankLiteVisitor : TankLiteBaseVisitor<TlValue>
{
    public Dictionary<string, TlValue> Variables { get; set; } = new();

    public override TlValue VisitFncall([NotNull] TankLiteParser.FncallContext context)
    {
        var breadcrumbs = context.deepIdent().IDENT().Select(i => i.GetText()).ToArray();

        var value = Variables.Get(breadcrumbs);
        var parent = breadcrumbs.Length > 1 ? (TlObj)Variables.Get(breadcrumbs[..^1]) : null;

        if (value is null) 
        {
            return new TlError(
                LanguageManager
                    .Get(LanguageName.TankLiteVariableDoesNotExist)
                    .Replace("{name}", string.Join('.', breadcrumbs))
            ); 
        }

        if (!value.Type.StartsWith("func"))
        {
            return new TlError(
                LanguageManager
                    .Get(LanguageName.TankLiteVariableNotFunction)
                    .Replace("{name}", string.Join('.', breadcrumbs))
                    .Replace("{type}", value.Type)
            );
        }

        var args = context.expr().Select(Visit).ToArray();
        var fn = (TlFunc)value;

        var fnArgs = new TlFuncArgs
        {
            Args = args,
            Parent = parent
        };

        return fn.Value(fnArgs);
    }

    public override TlValue VisitConstantExpression([NotNull] TankLiteParser.ConstantExpressionContext context)
    {
        var ctx = context.constant();

        if (ctx.BOOL() is { } b)
        {
            return new TlBool(b.GetText() == "true");
        }

        if (ctx.FLOAT() is { } f)
        {
            return new TlFloat(float.Parse(f.GetText(), CultureInfo.InvariantCulture));
        }

        if (ctx.INT() is { } i)
        {
            return new TlInt(int.Parse(i.GetText(), CultureInfo.InvariantCulture));
        }

        if (ctx.STRING() is { } s)
        {
            return new TlString(s.GetText()[1..^1]);
        }

        return new TlError("Shit happened");
    }

    public override TlValue VisitIdentifierExpression([NotNull] TankLiteParser.IdentifierExpressionContext context)
    {
        var breadcrumbs = context.deepIdent().IDENT().Select(i => i.GetText()).ToList();

        var value = Variables.Get(breadcrumbs);

        if (value is null)
        {
            return new TlError(
                LanguageManager
                    .Get(LanguageName.TankLiteVariableDoesNotExist)
                    .Replace("{name}", string.Join('.', breadcrumbs))
            );
        }

        return value;
    }

    public override TlValue VisitMultiplicationExpression([NotNull] TankLiteParser.MultiplicationExpressionContext context)
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

    public override TlValue VisitAdditiveExpression([NotNull] TankLiteParser.AdditiveExpressionContext context)
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

    public override TlValue VisitAssignment([NotNull] TankLiteParser.AssignmentContext context)
    {
        Variables[context.IDENT().GetText()] = Visit(context.expr());

        return new TlVoid();
    }

    public override TlValue VisitReassignment([NotNull] TankLiteParser.ReassignmentContext context)
    {
        var value = Visit(context.expr());
        var breadcrumbs = context.deepIdent().IDENT().Select(i => i.GetText()).ToList();

        Variables.Set(breadcrumbs, value);

        return new TlVoid();
    }

    public override TlValue VisitConstructorExpression([NotNull] TankLiteParser.ConstructorExpressionContext context)
    {
        var name = context.IDENT().GetText();

        if (!Variables.TryGetValue(name, out var value) || value.Type != "object")
        {
            return new TlError(
                LanguageManager
                    .Get(LanguageName.TankLiteClassDoesNotExist)
                    .Replace("{name}", name)
            );
        }

        var constructor = ((TlFunc)((TlObj)value).Value[".ctor"]).Value;
        var args = context.expr().Select(Visit).ToArray();

        var fnArgs = new TlFuncArgs
        {
            Args = args
        };

        return constructor(fnArgs);
    }
}
