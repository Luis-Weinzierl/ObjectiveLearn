using ObjectiveLearn.Models;
using TankLite.Models;
using TankLite.Values;
using Shared.Localization;

namespace ObjectiveLearn.Shared;

public class TriangleHelpers
{
    public static TankLiteValue Constructor(TankLiteFuncArgs args)
    {
        if (args.Args.Length != 8 && args.Args.Length != 4 && args.Args.Length != 5 && args.Args.Length != 9)
        {
            return new TankLiteError(
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
            if (arg.Type != TankLiteName.Int)
            {
                return new TankLiteError(
                    LanguageManager
                        .Get(LanguageName.ErrorTypeInferenceCtor)
                        .Replace("{i}", i.ToString())
                        .Replace("{type}", arg.Type)
                );
            }
        }

        return ShapeHelpers.CreateShape(args, TankLiteName.TriangleType);
    }
}
