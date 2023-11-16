using ObjectiveLearn.Models;
using TankLite.Models;
using TankLite.Values;
using Shared.Localization;

namespace ObjectiveLearn.Shared;

public class EllipseHelpers
{
    public static TlValue Constructor(TlFuncArgs args)
    {
        if (args.Args.Length != 8 && args.Args.Length != 4 && args.Args.Length != 5 && args.Args.Length != 9)
        {
            return new TlError(
                LanguageManager
                    .Get(LanguageName.ErrorExpectedArgs)
                    .Replace("{expected}", "4, 5, 8 {or} 9")
                    .Replace("{or}", LanguageManager.Get(LanguageName.ErrorOr))
                    .Replace("{received}", args.Args.Length.ToString())
            );
        }

        for (var i = 0; i < args.Args.Length; i++)
        {
            var arg = args.Args[i];
            if (arg.Type != TlName.Int)
            {
                return new TlError(
                    LanguageManager
                        .Get(LanguageName.ErrorTypeInferenceCtor)
                        .Replace("{i}", i.ToString())
                        .Replace("{type}", arg.Type)
                );
            }
        }

        return ShapeHelpers.CreateShape(args, TlName.EllipseType);
    }
}
