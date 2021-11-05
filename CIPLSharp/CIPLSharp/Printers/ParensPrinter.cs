using System;
using System.Collections.Generic;
using System.Text;

namespace CIPLSharp.Printers
{
    public class ParensPrinter : AstPrinter, Expr.IVisitor<string>, Statement.IVisitor<string>
    {
        private int nestingLevel;

        public override string PrintExpression(Expr expr)
        {
            return expr.Accept(this);
        }

        public override string PrintStatement(Statement statement)
        {
            return statement.Accept(this);
        }

        public override string PrintStatements(List<Statement> statements)
        {
            var sb = new StringBuilder();
            foreach (var statement in statements)
                AppendNesting(sb).Append(PrintStatement(statement)).Append('\n');

            return sb.ToString();
        }

        private StringBuilder AppendNesting(StringBuilder sb)
        {
            for (var i = 0; i < nestingLevel; i++)
                sb.Append("    ");
            return sb;
        }

        private string Parenthesize(string name, params Expr[] exprs)
        {
            var sb = new StringBuilder();

            sb.Append('(').Append(name);

            foreach (var expr in exprs)
            {
                sb.Append(' ');
                sb.Append(expr.Accept(this));
            }

            sb.Append(')');
            return sb.ToString();
        }
        
        private string Parenthesize(string name, List<Statement> statements)
        {
            var sb = new StringBuilder();

            sb.Append('(').Append(name).Append('\n');
            
            nestingLevel++;
            sb.Append(PrintStatements(statements));
            nestingLevel--;
            AppendNesting(sb).Append(')');
            
            return sb.ToString();
        }

        private string Parenthesize(string name, Expr condition, params (string, Statement)[] branches)
        {
            var sb = new StringBuilder();
            sb.Append('(').Append(name);
            sb.Append(' ').Append(condition.Accept(this));
            
            foreach (var (branchName, statement) in branches)
            {
                sb.Append(" (").Append(branchName).Append(' ');
                sb.Append(PrintStatement(statement));
            }

            sb.Append(')');
            return sb.ToString();
        }

        public string VisitLogicalExpr(Expr.Logical expr)
        {
            return Parenthesize(expr.OperatorToken.Lexeme, expr.Left, expr.Right);
        }

        public string VisitBinaryExpr(Expr.Binary expr)
        {
            return Parenthesize(expr.OperatorToken.Lexeme, expr.Left, expr.Right);
        }

        public string VisitCallExpr(Expr.Call expr)
        {
            var sb = new StringBuilder();

            sb.Append("(call ").Append(expr.Callee.Accept(this));

            foreach (var arg in expr.Arguments)
            {
                sb.Append(' ');
                sb.Append(arg.Accept(this));
            }

            sb.Append(')');
            return sb.ToString();
        }

        public string VisitUnaryExpr(Expr.Unary expr)
        {
            return Parenthesize(expr.OperatorToken.Lexeme, expr.Right);
        }

        public string VisitLiteralExpr(Expr.Literal expr)
        {
            return Interpreter.Stringify(expr.Value);
        }

        public string VisitGroupingExpr(Expr.Grouping expr)
        {
            return Parenthesize("group", expr.Expression);
        }

        public string VisitVariableExpr(Expr.Variable expr)
        {
            return expr.Name.Lexeme;
        }

        public string VisitAssignExpr(Expr.Assign expr)
        {
            return Parenthesize("assign " + expr.Name.Lexeme, expr.Value);
        }

        public string VisitExpressionStatementStatement(Statement.ExpressionStatement statement)
        {
            return Parenthesize("expression", statement.Expression);
        }

        public string VisitProcedureStatement(Statement.Procedure statement)
        {
            var sb = new StringBuilder();

            sb.Append("(proc ").Append(statement.Name.Lexeme).Append(' ');
            
            if (statement.Parameters.Count > 0)
            {
                sb.Append("(params");
                foreach (var arg in statement.Parameters)
                {
                    sb.Append(' ');
                    sb.Append(arg.Lexeme);
                }

                sb.Append(") ");
            }

            sb.Append(Parenthesize("body", statement.Body));
            return sb.ToString();
        }

        public string VisitReturnStatement(Statement.Return statement)
        {
            return Parenthesize("return", statement.Value);
        }

        public string VisitVarStatement(Statement.Var statement)
        {
            return Parenthesize("let " + statement.Name.Lexeme, statement.Initializer);
        }
        
        public string VisitIfStatement(Statement.If statement)
        {
            if (statement.ElseBranch is not null)
                return Parenthesize("if", statement.Condition, ("then", statement.ThenBranch), ("else", statement.ElseBranch));
            return Parenthesize("if", statement.Condition, ("then", statement.ThenBranch));
        }

        public string VisitWhileStatement(Statement.While statement)
        {
            return Parenthesize("while", statement.Condition, ("body", statement.Body));
        }

        public string VisitBreakStatement(Statement.Break statement)
        {
            return "break";
        }

        public string VisitPassStatement(Statement.Pass statement)
        {
            return "pass";
        }

        public string VisitBlockStatement(Statement.Block statement)
        {
            return Parenthesize("block", statement.Statements);
        }
    }
}