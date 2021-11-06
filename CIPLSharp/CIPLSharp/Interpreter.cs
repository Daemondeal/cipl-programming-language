using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using CIPLSharp.Runtime;
using static CIPLSharp.TokenType;

namespace CIPLSharp
{
    public class Interpreter : Expr.IVisitor<object>, Statement.IVisitor<object>
    {
        public readonly Environment Globals = new Environment();
        private readonly Dictionary<Expr, int> locals = new();
        private Environment environment;

        public Interpreter()
        {
            environment = Globals;
            
            Globals.Define("clock", new ClockProcedure());
            Globals.Define("print", new PrintProcedure());
            Globals.Define("to_string", new ToStringProcedure());
        }
        
        public void Interpret(List<Statement> statements)
        {
            try
            {
                foreach (var statement in statements)
                    Execute(statement);
            }
            catch (RuntimeError error)
            {
                Cipl.RuntimeError(error);
            }
        }

        private void Execute(Statement statement)
        {
            statement.Accept(this);
        }

        public void ExecuteBlock(List<Statement> statements, Environment environment)
        {
            var prev = this.environment;

            try
            {
                this.environment = environment;

                foreach (var statement in statements)
                {
                    Execute(statement);
                    if (shouldBreak) break;
                }
            }
            finally
            {
                this.environment = prev;
            }
        }
        
        public static string Stringify(object obj)
        {
            switch (obj)
            {
                case null:
                    return "null";
                case bool b:
                    return b ? "true" : "false";
                case double num:
                {
                    var text = num.ToString(CultureInfo.InvariantCulture);
                    return text.EndsWith(".0") ? text.Remove(text.Length - 2, 2) : text;
                }
                default:
                    return obj.ToString();
            }
        }

        private static bool IsTruthy(object obj)
        {
            return obj switch
            {
                null => false,
                bool b => b,
                _ => true
            };
        }
        
        private static bool IsEqual(object a, object b)
        {
            if (a is null && b is null) return true;
            if (a is null || b is null) return false;
            
            // double.NaN.Equals(double.NaN) is true, so we handle nan equality manually
            if (a is double.NaN || b is double.NaN) return false;
            
            return a.Equals(b);
        }

        private static void CheckNumberOperands(Token op, params object[] operands)
        {
            if (operands.Any(obj => obj is not double))
                throw new RuntimeError(op, "Operands must be numbers");
        }

        private object Evaluate(Expr expression)
        {
            return expression.Accept(this);
        }
        
        // Resolver
        public void Resolve(Expr expr, int depth)
        {
            locals[expr] = depth;
        }

        private object LookUpVariable(Token name, Expr expr)
        {
            if (locals.TryGetValue(expr, out var distance))
            {
                return environment.GetAt(distance, name.Lexeme);
            }
            else
            {
                return Globals.Get(name);
            }
        }

        public object VisitLogicalExpr(Expr.Logical expr)
        {
            var left = Evaluate(expr.Left);

            if (expr.OperatorToken.Type == OR)
            {
                if (IsTruthy(left)) 
                    return left;
            }
            else if (expr.OperatorToken.Type == AND)
            {
                if (!IsTruthy(left))
                    return left;
            }

            return Evaluate(expr.Right);
        }

        public object VisitBinaryExpr(Expr.Binary expr)
        {
            var left = Evaluate(expr.Left);
            var right = Evaluate(expr.Right);

            switch (expr.OperatorToken.Type)
            {
                case PLUS:
                    CheckNumberOperands(expr.OperatorToken, left, right);
                    return (double) left + (double) right;
                case TILDE:
                    if (left is string && right is string)
                        return (string) left + (string) right;
                    throw new RuntimeError(expr.OperatorToken, "Operands must be Strings");
                case MINUS:
                    CheckNumberOperands(expr.OperatorToken, left, right);
                    return (double) left - (double) right;
                case SLASH:
                    CheckNumberOperands(expr.OperatorToken, left, right);
                    return (double) left / (double) right;
                case STAR:
                    CheckNumberOperands(expr.OperatorToken, left, right);
                    return (double) left * (double) right;
                case GREATER:
                    CheckNumberOperands(expr.OperatorToken, left, right);
                    return (double) left > (double) right;
                case GREATER_EQUAL:
                    CheckNumberOperands(expr.OperatorToken, left, right);
                    return (double) left >= (double) right;
                case LESS:
                    CheckNumberOperands(expr.OperatorToken, left, right);
                    return (double) left < (double) right;
                case LESS_EQUAL:
                    CheckNumberOperands(expr.OperatorToken, left, right);
                    return (double) left <= (double) right;
                case EQUAL_EQUAL:
                    return IsEqual(left, right);
                case BANG_EQUAL:
                    return !IsEqual(left, right);
            }
            
            // Unreachable
            throw new RuntimeError(expr.OperatorToken, "Got to unreachable code. This could be a parser error");
        }

        public object VisitCallExpr(Expr.Call expr)
        {
            var callee = Evaluate(expr.Callee);

            var arguments = expr.Arguments.Select(Evaluate).ToList();

            if (callee is not ICiplCallable callable)
                throw new RuntimeError(expr.Paren, "Can only call procedures and classes");

            if (arguments.Count != callable.Arity())
                throw new RuntimeError(expr.Paren, $"Expected {callable.Arity()} arguments but got {arguments.Count} for {Stringify(callable)}()");

            return callable.Call(this, arguments);
        }

        public object VisitUnaryExpr(Expr.Unary expr)
        {
            var val = Evaluate(expr.Right);

            switch (expr.OperatorToken.Type)
            {
                case MINUS:
                    CheckNumberOperands(expr.OperatorToken, val);
                    return -(double) val;
                case NOT:
                    return !IsTruthy(val);
            }

            // Unreachable
            throw new RuntimeError(expr.OperatorToken, "Got to unreachable code. This could be a parser error");
        }

        public object VisitLiteralExpr(Expr.Literal expr)
        {
            return expr.Value;
        }

        public object VisitGroupingExpr(Expr.Grouping expr)
        {
            return Evaluate(expr.Expression);
        }

        public object VisitVariableExpr(Expr.Variable expr)
        {
            return LookUpVariable(expr.Name, expr);
        }

        public object VisitAssignExpr(Expr.Assign expr)
        {
            var value = Evaluate(expr.Value);
            
            if (locals.TryGetValue(expr, out var dist))
            {
                environment.AssignAt(dist, expr.Name, value);
            }
            else
            {
                Globals.Assign(expr.Name, value);
            }
            
            return value;
        }

        // Statements

        public object VisitExpressionStatementStatement(Statement.ExpressionStatement statement)
        {
            Evaluate(statement.Expression);
            return null;
        }

        public object VisitProcedureStatement(Statement.Procedure statement)
        {
            var procedure = new CiplProcedure(statement, environment);
            environment.Define(statement.Name.Lexeme, procedure);
            return null;
        }

        public object VisitReturnStatement(Statement.Return statement)
        {
            object value = null;
            if (statement.Value != null) value = Evaluate(statement.Value);

            throw new Return(value);
        }

        public object VisitIfStatement(Statement.If statement)
        {
            if (IsTruthy(Evaluate(statement.Condition)))
                Execute(statement.ThenBranch);
            else if (statement.ElseBranch != null)
                Execute(statement.ElseBranch);

            return null;
        }

        public object VisitVarStatement(Statement.Var statement)
        {
            var value = Evaluate(statement.Initializer);
            environment.Define(statement.Name.Lexeme, value);
            return null;
        }

        private int loopDepth;
        private bool shouldBreak;
        
        public object VisitWhileStatement(Statement.While statement)
        {
            loopDepth++;
            
            while (IsTruthy(Evaluate(statement.Condition)) && !shouldBreak)
                Execute(statement.Body);

            loopDepth--;
            shouldBreak = false;
            
            return null;
        }

        public object VisitBreakStatement(Statement.Break statement)
        {
            if (loopDepth < 0)
                throw new RuntimeError(statement.BreakToken, "Can't break outside loops");

            shouldBreak = true;
            return null;
        }

        public object VisitPassStatement(Statement.Pass statement)
        {
            return null;
        }

        public object VisitBlockStatement(Statement.Block statement)
        {
            ExecuteBlock(statement.Statements, new Environment(environment));
            return null;
        }
    }
}