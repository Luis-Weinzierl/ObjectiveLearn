﻿using System;
using System.Collections.Generic;
using System.Text;

namespace TankLite.Values;

public abstract class TLValue
{
    public virtual bool IsReadonly { get; set; } = false;
    public abstract string Type { get; set; }

    public virtual TLValue Add(TLValue _)
    {
        return new TLError(
                    LanguageManager
                        .Get(LanguageName.TankLiteCannotOperate)
                        .Replace("{type}", Type)
                );
    }

    public virtual TLValue Subtract(TLValue _)
    {
        return new TLError(
                    LanguageManager
                        .Get(LanguageName.TankLiteCannotOperate)
                        .Replace("{type}", Type)
                );
    }

    public virtual TLValue Multiply(TLValue _)
    {
        return new TLError(
                    LanguageManager
                        .Get(LanguageName.TankLiteCannotOperate)
                        .Replace("{type}", Type)
                );
    }

    public virtual TLValue Divide(TLValue _)
    {
        return new TLError(
                    LanguageManager
                        .Get(LanguageName.TankLiteCannotOperate)
                        .Replace("{type}", Type)
                );
    }

    public override string ToString()
    {
        return Type;
    }
}
