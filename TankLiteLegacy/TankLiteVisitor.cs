using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TankLite;

public class TankLiteVisitor : TankLiteBaseVisitor<ITLVariable>
{
    public override ITLVariable VisitFncall([NotNull] TankLiteParser.FncallContext context)
    {
        Console.WriteLine(context.IDENT());
        return base.VisitFncall(context);
    }
}
