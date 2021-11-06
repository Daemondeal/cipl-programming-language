using System.Collections.Generic;

namespace CIPLSharp
{
    public abstract class Expr
    {
        public interface IVisitor<T>
        {
            T VisitLogicalExpr(Logical expr);
            T VisitBinaryExpr(Binary expr);
            T VisitCallExpr(Call expr);
            T VisitGetExpr(Get expr);
            T VisitSetExpr(Set expr);
            T VisitSuperExpr(Super expr);
            T VisitThisExpr(This expr);
            T VisitUnaryExpr(Unary expr);
            T VisitLiteralExpr(Literal expr);
            T VisitGroupingExpr(Grouping expr);
            T VisitVariableExpr(Variable expr);
            T VisitAssignExpr(Assign expr);
        }

        public abstract T Accept<T>(IVisitor<T> visitor);

        public class Logical : Expr
        {
            public readonly Expr Left;
            public readonly Token OperatorToken;
            public readonly Expr Right;

            public Logical(Expr left, Token operatorToken, Expr right)
            {
                Left = left;
                OperatorToken = operatorToken;
                Right = right;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.VisitLogicalExpr(this);
            }
        }
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
        public class Call : Expr
        {
            public readonly Expr Callee;
            public readonly Token Paren;
            public readonly List<Expr> Arguments;

            public Call(Expr callee, Token paren, List<Expr> arguments)
            {
                Callee = callee;
                Paren = paren;
                Arguments = arguments;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.VisitCallExpr(this);
            }
        }
        public class Get : Expr
        {
            public readonly Expr Obj;
            public readonly Token Name;

            public Get(Expr obj, Token name)
            {
                Obj = obj;
                Name = name;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.VisitGetExpr(this);
            }
        }
        public class Set : Expr
        {
            public readonly Expr Obj;
            public readonly Token Name;
            public readonly Expr Value;

            public Set(Expr obj, Token name, Expr value)
            {
                Obj = obj;
                Name = name;
                Value = value;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.VisitSetExpr(this);
            }
        }
        public class Super : Expr
        {
            public readonly Token Keyword;
            public readonly Token Method;

            public Super(Token keyword, Token method)
            {
                Keyword = keyword;
                Method = method;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.VisitSuperExpr(this);
            }
        }
        public class This : Expr
        {
            public readonly Token Keyword;

            public This(Token keyword)
            {
                Keyword = keyword;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.VisitThisExpr(this);
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
