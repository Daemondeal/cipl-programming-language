namespace CIPLSharp
{
    public abstract class Expr
    {
        public interface IVisitor<T>
        {
            T VisitBinaryExpr(Binary expr);
            T VisitUnaryExpr(Unary expr);
            T VisitLiteralExpr(Literal expr);
            T VisitGroupingExpr(Grouping expr);
            T VisitVariableExpr(Variable expr);
            T VisitAssignExpr(Assign expr);
        }

        public abstract T Accept<T>(IVisitor<T> visitor);

        public class Binary : Expr
        {
            public readonly Expr Left;
            public readonly Token OperatorToken;
            public readonly Expr Right;

            public Binary(Expr left, Token operatorToken, Expr right)
            {
                Left = left;
                OperatorToken = operatorToken;
                Right = right;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.VisitBinaryExpr(this);
            }
        }
        public class Unary : Expr
        {
            public readonly Token OperatorToken;
            public readonly Expr Right;

            public Unary(Token operatorToken, Expr right)
            {
                OperatorToken = operatorToken;
                Right = right;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.VisitUnaryExpr(this);
            }
        }
        public class Literal : Expr
        {
            public readonly object Value;

            public Literal(object value)
            {
                Value = value;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.VisitLiteralExpr(this);
            }
        }
        public class Grouping : Expr
        {
            public readonly Expr Expression;

            public Grouping(Expr expression)
            {
                Expression = expression;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.VisitGroupingExpr(this);
            }
        }
        public class Variable : Expr
        {
            public readonly Token Name;

            public Variable(Token name)
            {
                Name = name;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.VisitVariableExpr(this);
            }
        }
        public class Assign : Expr
        {
            public readonly Token Name;
            public readonly Expr Value;

            public Assign(Token name, Expr value)
            {
                Name = name;
                Value = value;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.VisitAssignExpr(this);
            }
        }
    }
}
