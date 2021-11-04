namespace CIPLSharp
{
    public abstract class Statement
    {
        public interface IVisitor<T>
        {
            T VisitExpressionStatementStatement(ExpressionStatement statement);
            T VisitPrintStatement(Print statement);
            T VisitVarStatement(Var statement);
            T VisitBlockStatement(Block statement);
        }

        public abstract T Accept<T>(IVisitor<T> visitor);

        public class ExpressionStatement : Statement
        {
            public readonly Expr Expression;

            public ExpressionStatement(Expr expression)
            {
                Expression = expression;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.VisitExpressionStatementStatement(this);
            }
        }
        public class Print : Statement
        {
            public readonly Expr Expression;

            public Print(Expr expression)
            {
                Expression = expression;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.VisitPrintStatement(this);
            }
        }
        public class Var : Statement
        {
            public readonly Token Name;
            public readonly Expr Initializer;

            public Var(Token name, Expr initializer)
            {
                Name = name;
                Initializer = initializer;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.VisitVarStatement(this);
            }
        }
        public class Block : Statement
        {
            public readonly List<Statement> Statements;

            public Block(List<Statement> statements)
            {
                Statements = statements;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.VisitBlockStatement(this);
            }
        }
    }
}
