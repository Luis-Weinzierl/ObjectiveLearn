using ObjectiveLearn.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TankLite.Models;
using TankLite.Values;
using Shared.Localisation;

namespace ObjectiveLearn.Shared;

public class EllipseHelpers
{
    public static TLValue Constructor(TLFuncArgs args)
    {
        if (args.Args.Length != 8 && args.Args.Length != 4 && args.Args.Length != 5 && args.Args.Length != 9)
        {
            return new TLError(
                LanguageManager
                    .Get(LanguageName.ErrorExpectedArgs)
                    .Replace("{expected}", "4, 5, 8 {or} 9")
                    .Replace("{or}", LanguageManager.Get(LanguageName.ErrorOr))
                    .Replace("{received}", args.Args.Length.ToString())
            );
        }

        for (int i = 0; i < args.Args.Length; i++)
        {
            var arg = args.Args[i];
            if (arg.Type != TLName.Int)
            {
                return new TLError(
                    LanguageManager
                        .Get(LanguageName.ErrorTypeInferenceCtor)
                        .Replace("{i}", i.ToString())
                        .Replace("{type}", arg.Type)
                );
            }
        }

        return ShapeHelpers.CreateShape(args, TLName.EllipseType);
    }
}
