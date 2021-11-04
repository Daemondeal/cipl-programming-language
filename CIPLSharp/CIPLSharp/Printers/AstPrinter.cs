using System.Collections.Generic;

namespace CIPLSharp.Printers
{
    public abstract class AstPrinter
    {
        public abstract string PrintExpression(Expr expr);
        public abstract string PrintStatement(Statement statement);
        public abstract string PrintStatements(List<Statement> statements);
    }
}