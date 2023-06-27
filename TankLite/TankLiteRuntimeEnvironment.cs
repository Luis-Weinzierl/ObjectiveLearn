using Antlr4.Runtime;
using System.Collections.Generic;
using TankLite.Values;

namespace TankLite;

public class TankLiteRuntimeEnvironment
{
    public TankLiteVisitor Visitor { get; set; } = new();

    public TankLiteRuntimeEnvironment(Dictionary<string, TLValue> variables) 
    {
        Visitor.Variables = variables;
    }

    public void Execute(string input)
    {
        var inputStream = new AntlrInputStream(input);
        var tankLiteLexer = new TankLiteLexer(inputStream);
        var commonTokenStream = new CommonTokenStream(tankLiteLexer);
        var tankLiteParser = new TankLiteParser(commonTokenStream);
        var programContext = tankLiteParser.program();
        Visitor.Visit(programContext);
    }
}