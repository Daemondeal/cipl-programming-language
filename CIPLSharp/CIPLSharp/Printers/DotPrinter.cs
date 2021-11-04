// using System;
// using System.Collections.Generic;
// using System.Text;
// using CIPLSharp.Printers;
//
// namespace CIPLSharp
// {
//     public class DotPrinter : AstPrinter, Expr.IVisitor<string>, Statement.IVisitor<string>
//     {
//         public override string PrintExpression(Expr expr)
//         {
//             var sb = new StringBuilder();
//             sb.Append("digraph AST {\n");
//             // sb.Append("    edge[dir=\"back\"];\n");
//             sb.Append(expr.Accept(this));
//             sb.Append("}\n");
//             return sb.ToString();
//         }
//
//         public override string PrintStatement(Statement statement)
//         {
//             return statement.Accept(this);
//         }
//
//         public override string PrintStatements(List<Statement> statements)
//         {
//             var sb = new StringBuilder();
//             sb.Append("digraph AST {\n");
//             for (var i = 0; i < statements.Count; i++)
//             {
//                 sb.Append(PrintStatement(statements[i]));
//                 if (i < statements.Count - 1)
//                     sb.Append("    ").Append(GetUid(statements[i])).Append(" -> ").Append(GetUid(statements[i + 1])).Append('\n');
//
//             }
//             sb.Append("}\n");
//
//             return sb.ToString();
//         }
//
//         private string GetUid(Expr expr)
//         {
//             return "Expr_" + expr.GetHashCode();
//         }
//
//         private string GetUid(Statement statement)
//         {
//             return "Stmt_" + statement.GetHashCode();
//         }
//
//         public string Expand(string label, Expr parent, params Expr[] exprs)
//         {
//             var uid = GetUid(parent);
//
//             var sb = new StringBuilder();
//
//             sb.Append("    ").Append(uid).Append("[label=\"").Append(label).Append("\"];\n");
//             foreach (var expr in exprs)
//             {
//                 sb.Append(expr.Accept(this));
//                 var childUid = GetUid(expr);
//
//                 sb.Append("    ").Append(childUid).Append(" -> ").Append(uid).Append(";\n");
//             }
//
//             return sb.ToString();
//         }
//
//         public string Expand(string label, Statement parent, params Expr[] exprs)
//         {
//             var uid = GetUid(parent);
//             var sb = new StringBuilder();
//             sb.Append("    ").Append(uid).Append("[label=\"").Append(label).Append("\", shape=\"box\"];\n");
//
//             foreach (var expr in exprs)
//             {
//                 sb.Append(expr.Accept(this));
//                 var childUid = GetUid(expr);
//                 sb.Append("    ").Append(childUid).Append(" -> ").Append(uid).Append(";\n");
//             }
//
//             return sb.ToString();
//         }
//
//         public string VisitLogicalExpr(Expr.Logical expr)
//         {
//             throw new NotImplementedException();
//         }
//
//         public string VisitBinaryExpr(Expr.Binary expr)
//         {
//             return Expand(expr.OperatorToken.Lexeme, expr, expr.Left, expr.Right);
//         }
//
//         public string VisitCallExpr(Expr.Call expr)
//         {
//             throw new NotImplementedException();
//         }
//
//         public string VisitUnaryExpr(Expr.Unary expr)
//         {
//             return Expand(expr.OperatorToken.Lexeme, expr, expr.Right);
//         }
//
//         public string VisitLiteralExpr(Expr.Literal expr)
//         {
//             if (expr.Value == null) return Expand("null", expr);
//             return Expand(expr.Value.ToString(), expr);
//         }
//
//         public string VisitGroupingExpr(Expr.Grouping expr)
//         {
//             return Expand("group", expr, expr.Expression);
//         }
//
//         public string VisitVariableExpr(Expr.Variable expr)
//         {
//             return Expand(expr.Name.Lexeme, expr);
//         }
//
//         public string VisitAssignExpr(Expr.Assign expr)
//         {
//             return Expand("assign " + expr.Name.Lexeme, expr);
//         }
//
//         public string VisitExpressionStatementStatement(Statement.ExpressionStatement statement)
//         {
//             return Expand("expr", statement, statement.Expression);
//         }
//
//         public string VisitProcedureStatement(Statement.Procedure statement)
//         {
//             throw new NotImplementedException();
//         }
//
//         public string VisitIfStatement(Statement.If statement)
//         {
//             throw new NotImplementedException();
//         }
//
//         public string VisitVarStatement(Statement.Var statement)
//         {
//             return Expand("let " + statement.Name.Lexeme, statement, statement.Initializer);
//         }
//
//         public string VisitWhileStatement(Statement.While statement)
//         {
//             throw new NotImplementedException();
//         }
//
//         public string VisitBreakStatement(Statement.Break statement)
//         {
//             throw new NotImplementedException();
//         }
//
//         public string VisitPassStatement(Statement.Pass statement)
//         {
//             throw new NotImplementedException();
//         }
//
//         public string VisitBlockStatement(Statement.Block statement)
//         {
//             throw new NotImplementedException();
//             // return Expand("block", statement, statement.Statements);
//         }
//     }
// }