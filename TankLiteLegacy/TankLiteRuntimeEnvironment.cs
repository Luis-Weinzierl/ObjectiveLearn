using Antlr4.Runtime;

namespace TankLite;

public class TankLiteRuntimeEnvironment
{
    public void Execute(string input)
    {
        var inputStream = new AntlrInputStream(input);
        var tankLiteLexer = new TankLiteLexer(inputStream);
        var commonTokenStream = new CommonTokenStream(tankLiteLexer);
        var tankLiteParser = new TankLiteParser(commonTokenStream);
        var chatContext = tankLiteParser.program();
        var visitor = new TankLiteVisitor();
        visitor.Visit(chatContext);
    }
}