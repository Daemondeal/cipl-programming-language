using System;
using System.Collections.Generic;
using System.Linq;

namespace CIPLSharp
{
    public class Resolver : Expr.IVisitor<object>, Statement.IVisitor<object>
    {
        private readonly Interpreter interpreter;
        private readonly Stack<Dictionary<string, bool>> scopes = new();
        private ProcedureType currentProcedure = ProcedureType.NONE;
        private ClassType currentClass = ClassType.NONE;
        private bool isInLoop = false;
        
        private enum ProcedureType
        {
            NONE, 
            PROCEDURE,
            INITIALIZER,
            METHOD
        }

        private enum ClassType
        {
            NONE,
            CLASS,
            SUBCLASS
        }

        public Resolver(Interpreter interpreter)
        {
            this.interpreter = interpreter;
        }

        private void BeginScope()
        {
            scopes.Push(new Dictionary<string, bool>());
        }

        private void EndScope()
        {
            scopes.Pop();
        }

        private void Declare(Token name)
        {
            if (scopes.Count == 0)
                return;

            var scope = scopes.Peek();
            if (scope.ContainsKey(name.Lexeme))
                Cipl.Error(name, "There's already a variable with this name in this scope");
            
            scope[name.Lexeme] = false;
        }

        private void Define(Token name)
        {
            if (scopes.Count == 0)
                return;

            scopes.Peek()[name.Lexeme] = true;

        }

        public void Resolve(List<Statement> statements)
        {
            foreach (var statement in statements)
                Resolve(statement);
        }

        private void Resolve(Statement statement)
        {
            statement.Accept(this);
        }

        private void Resolve(Expr expr)
        {
            expr.Accept(this);
        }

        private void ResolveLocal(Expr expr, Token name)
        {
            for (var i = 0; i < scopes.Count; i++)
            {
                if (scopes.ElementAt(i).ContainsKey(name.Lexeme))
                {
                    interpreter.Resolve(expr, i);
                    return;
                }
            }
        }

        private void ResolveProcedure(Statement.Procedure procedure, ProcedureType type)
        {
            var lastProcedure = currentProcedure;
            currentProcedure = type;
            
            BeginScope();
            foreach (var param in procedure.Parameters)
            {
                Declare(param);
                Define(param);
            }
            
            Resolve(procedure.Body);
            EndScope();

            currentProcedure = lastProcedure;
        }

        public object VisitLogicalExpr(Expr.Logical expr)
        {
            Resolve(expr.Left);
            Resolve(expr.Right);
            return null;
        }

        public object VisitBinaryExpr(Expr.Binary expr)
        {
            Resolve(expr.Left);
            Resolve(expr.Right);
            return null;
        }

        public object VisitCallExpr(Expr.Call expr)
        {
            Resolve(expr.Callee);
            foreach (var arg in expr.Arguments)
                Resolve(arg);

            return null;
        }

        public object VisitGetExpr(Expr.Get expr)
        {
            Resolve(expr.Obj);
            return null;
        }

        public object VisitSetExpr(Expr.Set expr)
        {
            Resolve(expr.Value);
            Resolve(expr.Obj);

            return null;
        }

        public object VisitSuperExpr(Expr.Super expr)
        {
            if (currentClass == ClassType.NONE)
                Cipl.Error(expr.Keyword, "Can't use 'super' outside of a class");
            else if (currentClass == ClassType.CLASS)
                Cipl.Error(expr.Keyword, "Can't user 'super' in a class with no superclass.");
            
            ResolveLocal(expr, expr.Keyword);
            return null;
        }

        public object VisitThisExpr(Expr.This expr)
        {
            if (currentClass == ClassType.NONE)
            {
                Cipl.Error(expr.Keyword, "'this' cant be used outside of a class");
            }
            ResolveLocal(expr, expr.Keyword);
            return null;
        }

        public object VisitUnaryExpr(Expr.Unary expr)
        {
            Resolve(expr.Right);

            return null;
        }

        public object VisitLiteralExpr(Expr.Literal expr)
        {
            return null;
        }

        public object VisitGroupingExpr(Expr.Grouping expr)
        {
            Resolve(expr.Expression);
            return null;
        }

        public object VisitVariableExpr(Expr.Variable expr)
        {
            if (scopes.Count > 0 && scopes.Peek().TryGetValue(expr.Name.Lexeme, out var val) && val == false)
            {
                Cipl.Error(expr.Name, "Can't read local variable in its own initializer");
            }

            ResolveLocal(expr, expr.Name);
            return null;
        }

        public object VisitAssignExpr(Expr.Assign expr)
        {
            Resolve(expr.Value);
            ResolveLocal(expr, expr.Name);
            return null;
        }

        public object VisitExpressionStatementStatement(Statement.ExpressionStatement statement)
        {
            Resolve(statement.Expression);
            return null;
        }

        public object VisitProcedureStatement(Statement.Procedure statement)
        {
            Declare(statement.Name);
            Define(statement.Name);

            ResolveProcedure(statement, ProcedureType.PROCEDURE);
            return null;
        }

        public object VisitReturnStatement(Statement.Return statement)
        {
            if (currentProcedure == ProcedureType.NONE)
                Cipl.Error(statement.Keyword, "Can't return from top level code");
            
            if (statement.Value != null)
            {
                if (currentProcedure == ProcedureType.INITIALIZER)
                    Cipl.Error(statement.Keyword, "Can't return a value from an initializer");
                Resolve(statement.Value);
            }

            return null;
        }

        public object VisitIfStatement(Statement.If statement)
        {
            Resolve(statement.Condition);
            Resolve(statement.ThenBranch);
            if (statement.ElseBranch != null)
                Resolve(statement.ElseBranch);

            return null;
        }

        public object VisitVarStatement(Statement.Var statement)
        {
            Declare(statement.Name);
            if (statement.Initializer != null)
            {
                Resolve(statement.Initializer);
            }

            Define(statement.Name);
            return null;
        }

        public object VisitWhileStatement(Statement.While statement)
        {
            var lastLoopStatus = isInLoop;

            isInLoop = true;
            Resolve(statement.Condition);
            Resolve(statement.Body);
            isInLoop = lastLoopStatus;
            
            return null;
        }

        public object VisitBreakStatement(Statement.Break statement)
        {
            if (!isInLoop)
                Cipl.Error(statement.BreakToken, "Can't break outside loops");

            return null;
        }

        public object VisitPassStatement(Statement.Pass statement)
        {
            return null;
        }

        public object VisitBlockStatement(Statement.Block statement)
        {
            BeginScope();
            Resolve(statement.Statements);
            EndScope();
            return null;
        }

        public object VisitClassStatement(Statement.Class statement)
        {
            var enclosingClass = currentClass;
            currentClass = ClassType.CLASS;
            
            
            
            Declare(statement.Name);
            Define(statement.Name);
            
            if (statement.Superclass is not null && statement.Name.Lexeme == statement.Superclass.Name.Lexeme)
                Cipl.Error(statement.Superclass.Name, "A class can't inherit from itself.");
                
            if (statement.Superclass is not null)
            {
                currentClass = ClassType.SUBCLASS;
                Resolve(statement.Superclass);

                BeginScope();
                scopes.Peek()["super"] = true;
            }
            
            BeginScope();
            scopes.Peek()["this"] = true;

            foreach (var method in statement.Methods)
            {
                var decl = ProcedureType.METHOD;
                if (method.Name.Lexeme.Equals(CiplClass.InitName))
                    decl = ProcedureType.INITIALIZER;
                
                ResolveProcedure(method, decl);
            }
            
            EndScope();
            
            if (statement.Superclass is not null)
                EndScope();

            currentClass = enclosingClass;
            
            return null;
        }
    }
}