using System.Collections.Generic;

namespace CIPLSharp
{
    public abstract class Statement
    {
        public interface IVisitor<T>
        {
            T VisitExpressionStatementStatement(ExpressionStatement statement);
            T VisitProcedureStatement(Procedure statement);
            T VisitReturnStatement(Return statement);
            T VisitIfStatement(If statement);
            T VisitVarStatement(Var statement);
            T VisitWhileStatement(While statement);
            T VisitBreakStatement(Break statement);
            T VisitPassStatement(Pass statement);
            T VisitBlockStatement(Block statement);
            T VisitClassStatement(Class statement);
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
        public class Procedure : Statement
        {
            public readonly Token Name;
            public readonly List<Token> Parameters;
            public readonly List<Statement> Body;

            public Procedure(Token name, List<Token> parameters, List<Statement> body)
            {
                Name = name;
                Parameters = parameters;
                Body = body;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.VisitProcedureStatement(this);
            }
        }
        public class Return : Statement
        {
            public readonly Token Keyword;
            public readonly Expr Value;

            public Return(Token keyword, Expr value)
            {
                Keyword = keyword;
                Value = value;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.VisitReturnStatement(this);
            }
        }
        public class If : Statement
        {
            public readonly Expr Condition;
            public readonly Statement ThenBranch;
            public readonly Statement ElseBranch;

            public If(Expr condition, Statement thenBranch, Statement elseBranch)
            {
                Condition = condition;
                ThenBranch = thenBranch;
                ElseBranch = elseBranch;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.VisitIfStatement(this);
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
        public class While : Statement
        {
            public readonly Expr Condition;
            public readonly Statement Body;

            public While(Expr condition, Statement body)
            {
                Condition = condition;
                Body = body;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.VisitWhileStatement(this);
            }
        }
        public class Break : Statement
        {
            public readonly Token BreakToken;

            public Break(Token breakToken)
            {
                BreakToken = breakToken;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.VisitBreakStatement(this);
            }
        }
        public class Pass : Statement
        {
            public readonly Token PassToken;

            public Pass(Token passToken)
            {
                PassToken = passToken;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.VisitPassStatement(this);
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
        public class Class : Statement
        {
            public readonly Token Name;
            public readonly List<Statement.Procedure> Methods;

            public Class(Token name, List<Statement.Procedure> methods)
            {
                Name = name;
                Methods = methods;
            }

            public override T Accept<T>(IVisitor<T> visitor)
            {
                return visitor.VisitClassStatement(this);
            }
        }
    }
}
