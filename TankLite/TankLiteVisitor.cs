using Antlr4.Runtime.Misc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TankLite.Extensions;
using TankLite.Models;
using TankLite.Values;

namespace TankLite;

public class TankLiteVisitor : TankLiteBaseVisitor<TLValue>
{
    public Dictionary<string, TLValue> Variables { get; set; } = new();

    public override TLValue VisitFncall([NotNull] TankLiteParser.FncallContext context)
    {
        var breadcrumbs = context.deepIdent().IDENT().Select(i => i.GetText()).ToArray();

        var value = Variables.Get(breadcrumbs);
        var parent = breadcrumbs.Length > 1 ? (TLObj)Variables.Get(breadcrumbs[..^1]) : null;

        if (value is not { }) 
        {
            return new TLError($"Variable {string.Join('.', breadcrumbs)} existiert nicht."); 
        }

        if (!value.Type.StartsWith("func"))
        {
            return new TLError($"Variable {string.Join('.', breadcrumbs)} kann nicht als Funktion ausgeführt werden, da sie vom Typ {value.Type} ist.");
        }

        var args = context.expr().Select(Visit).ToArray();
        var fn = (TLFunc)value;

        var fnArgs = new TLFuncArgs()
        {
            Args = args,
            Parent = parent
        };

        return fn.Value(fnArgs);
    }

    public override TLValue VisitConstantExpression([NotNull] TankLiteParser.ConstantExpressionContext context)
    {
        var ctx = context.constant();

        if (ctx.BOOL() is { } b)
        {
            return new TLBool(b.GetText() == "true");
        }

        if (ctx.FLOAT() is { } f)
        {
            return new TLFloat(float.Parse(f.GetText(), CultureInfo.InvariantCulture));
        }

        if (ctx.INT() is { } i)
        {
            return new TLInt(int.Parse(i.GetText(), CultureInfo.InvariantCulture));
        }

        if (ctx.STRING() is { } s)
        {
            return new TLString(s.GetText()[1..^1]);
        }

        return new TLError("Shit happened");
    }

    public override TLValue VisitIdentifierExpression([NotNull] TankLiteParser.IdentifierExpressionContext context)
    {
        var breadcrumbs = context.deepIdent().IDENT().Select(i => i.GetText()).ToList();

        var value = Variables.Get(breadcrumbs);

        if (value is not { })
        {
            return new TLError($"Variable {string.Join('.', breadcrumbs)} existiert nicht.");
        }

        return value;
    }

    public override TLValue VisitMultiplicationExpression([NotNull] TankLiteParser.MultiplicationExpressionContext context)
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

    public override TLValue VisitAdditiveExpression([NotNull] TankLiteParser.AdditiveExpressionContext context)
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

    public override TLValue VisitAssignment([NotNull] TankLiteParser.AssignmentContext context)
    {
        Variables[context.IDENT().GetText()] = Visit(context.expr());

        return new TLVoid();
    }

    public override TLValue VisitConstructorExpression([NotNull] TankLiteParser.ConstructorExpressionContext context)
    {
        var name = context.IDENT().GetText();

        if (!Variables.TryGetValue(name, out TLValue value) || value.Type != "object")
        {
            return new TLError($"Klasse {name} existiert nicht.");
        }

        var constructor = ((TLFunc)((TLObj)value).Value[".ctor"]).Value;
        var args = context.expr().Select(Visit).ToArray();

        var fnArgs = new TLFuncArgs()
        {
            Args = args
        };

        return constructor(fnArgs);
    }
}
