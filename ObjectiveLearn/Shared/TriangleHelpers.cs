using ObjectiveLearn.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TankLite.Models;
using TankLite.Values;

namespace ObjectiveLearn.Shared;

public class TriangleHelpers
{
    public static TLValue Constructor(TLFuncArgs args)
    {
        if (args.Args.Length != 8 && args.Args.Length != 4 && args.Args.Length != 5 && args.Args.Length != 9)
        {
            return new TLError($"4, 5, 8 oder 9 Argumente wurden erwarted, aber {args.Args.Length} erhalten.");
        }

        for (int i = 0; i < args.Args.Length; i++)
        {
            var arg = args.Args[i];
            if (arg.Type != TLName.Int)
            {
                return new TLError($"Das {i}. Argument des Konstruktors sollte vom Typ int sein, ist aber {arg.Type}.");
            }
        }

        return ShapeHelpers.CreateShape(args, TLName.TriangleType);
    }
}
