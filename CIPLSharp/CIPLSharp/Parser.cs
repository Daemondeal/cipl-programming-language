using System;
using System.Collections.Generic;
using System.Linq;
using static CIPLSharp.TokenType;

namespace CIPLSharp
{
    public class Parser
    {
        private class ParseError : Exception
        {
        }

        private readonly List<Token> tokens;
        private int current = 0;

        public Parser(List<Token> tokens)
        {
            this.tokens = tokens;
        }

        public Expr ParseExpression()
        {
            try
            {
                return Expression();
            }
            catch (ParseError)
            {
                return null;
            }
        }

        public List<Statement> Parse()
        {
            var statements = new List<Statement>();
            
            while (!IsAtEnd())
                statements.Add(Declaration());

            return statements;
        }
        
        // Navigating Tokens

        private bool IsAtEnd()
        {
            return Peek().Type == EOF;
        }

        private Token Peek()
        {
            return tokens[current];
        }

        private Token Previous()
        {
            return tokens[current - 1];
        }

        private Token Advance()
        {
            if (!IsAtEnd()) current++;
            return Previous();
        }

        private bool Check(TokenType type)
        {
            if (IsAtEnd()) return false;
            return Peek().Type == type;
        }

        private bool Match(params TokenType[] types)
        {
            if (!types.Any(Check)) return false;
            
            Advance();
            return true;
        }
        
        private Token Consume(TokenType type, string errorMessage)
        {
            if (Check(type)) return Advance();

            throw Error(Peek(), errorMessage);
        }
        
        // Statements
        private Statement Declaration()
        {
            try
            {
                if (Match(LET))
                    return VariableDeclaration();
                if (Match(PROC))
                    return Procedure("procedure");
                return Statement();
            }
            catch (ParseError _)
            {
                Synchronize();
                return null;
            }
        }

        private Statement Procedure(string kind)
        {
            var name = Consume(IDENTIFIER, $"Expect {kind} name");
            Consume(LEFT_PAREN, $"Expect '(' after {kind} name");

            var parameters = new List<Token>();

            if (!Check(RIGHT_PAREN))
            {
                do
                {
                    if (parameters.Count >= 255)
                    {
                        Error(Peek(), "Can't have more than 255 parameters");
                    }

                    parameters.Add(Consume(IDENTIFIER, "Expect parameter name"));
                } while (Match(COMMA));
            }
            
            Consume(RIGHT_PAREN, "Expect ')' after parameters");
            Consume(COLON, "Expect ':' after parameters");
            Consume(LINE_END, $"Expect line end before {kind} body");
            Consume(INDENT, $"Expect indent before {kind} body");

            var body = Block();
            return new Statement.Procedure(name, parameters, body);
        }

        private Statement VariableDeclaration()
        {
            var name = Consume(IDENTIFIER, "Expected variable name.");

            Expr initializer = new Expr.Literal(null);
            if (Match(EQUAL))
            {
                initializer = Expression();
            }

            Consume(LINE_END, "Expected line end after variable declaration");
            return new Statement.Var(name, initializer);
        }
        
        private Statement Statement()
        {
            if (Match(IF)) 
                return IfStatement();

            if (Match(WHILE))
                return WhileStatement();

            if (Match(REPEAT))
                return RepeatStatement();

            if (Match(FOR))
                return ForStatement();

            if (Match(BREAK))
                return BreakStatement();
            
            if (Match(PASS))
                return PassStatement();
            
            if (Match(RETURN))
                return ReturnStatement();

            if (Match(INDENT))
                return new Statement.Block(Block());

            return ExpressionStatement();
        }

        private Statement ReturnStatement()
        {
            var keyword = Previous();
            Expr value = null;

            if (!Check(LINE_END))
                value = Expression();

            Consume(LINE_END, "Expected line end after return");
            return new Statement.Return(keyword, value);
        }

        private Statement BreakStatement()
        {
            var token = Previous();
            Consume(LINE_END, "Expected line end after break statement");
            return new Statement.Break(token);
        }

        private Statement IfStatement()
        {
            var condition = Expression();
            Consume(COLON, "Expected ':' after if condition");

            // Skip line end if needed
            Match(LINE_END);

            var thenBranch = Statement();
            Statement elseBranch = null;
            
            if (Match(ELSE))
            {
                Consume(COLON, "Expected ':' after else condition");
                Match(LINE_END);
                elseBranch = Statement();
            }
            return new Statement.If(condition, thenBranch, elseBranch);
        }

        private Statement WhileStatement()
        {
            var condition = Expression();
            Consume(COLON, "Expected ':' after while condition");

            // Skip line end if needed
            Match(LINE_END);

            var body = Statement();

            return new Statement.While(condition, body);
        }

        private Statement RepeatStatement()
        {
            var name = Consume(IDENTIFIER, "Expected variable name after repeat");
            var limit = Expression();
            Consume(COLON, "Expected colon after repeat token");
            
            // Skip line end if needed
            Match(LINE_END);

            var body = Statement();
            var condition = new Expr.Binary(new Expr.Variable(name), new Token(LESS, "<", "", name.Line), limit);

            return new Statement.Block(new List<Statement>
            {
                new Statement.Var(name, new Expr.Literal((double)0)),
                new Statement.While(condition, new Statement.Block(new List<Statement>
                {
                    body,
                    GetIncrement(name)
                }))
            });
        }

        private Statement ForStatement()
        {
            var name = Consume(IDENTIFIER, "Expected variable name after for");
            Consume(IN, "Expected 'in' after iteration value");

            var start = Expression();

            var comparison = new Token(LESS, "<", "", name.Line);
            if (Match(THREE_DOTS))
                comparison = new Token(LESS_EQUAL, "<=", "", name.Line);
            else
                Consume(DOT_DOT, "Expected '..' or '...' after start");
            
            var end = Expression();

            Consume(COLON, "Expected ':' after for");
            
            // Skip line end if needed
            Match(LINE_END);
            
            var body = Statement();

            var condition = new Expr.Binary(new Expr.Variable(name), comparison, end);

            return new Statement.Block(new List<Statement>
            {
                new Statement.Var(name, start),
                new Statement.While(condition, new Statement.Block(new List<Statement>
                {
                    body,
                    GetIncrement(name)
                }))
            });
        }

        private Statement GetIncrement(Token name)
        {
            var plus = new Token(PLUS, "+", "", name.Line);
            return new Statement.ExpressionStatement(
                new Expr.Assign(name, 
                    new Expr.Binary(
                        new Expr.Variable(name), 
                        plus, 
                        new Expr.Literal((double)1))
                    )
                );
        }

        private List<Statement> Block()
        {
            var statements = new List<Statement>();

            while (!Check(DEDENT) && !IsAtEnd())
            {
                statements.Add(Declaration());
            }

            Consume(DEDENT, "Expected dedent after block.");
            return statements;
        }

        private Statement PassStatement()
        {
            Consume(LINE_END, "Expected line end after pass");
            return new Statement.Pass(Peek());
        }

        private Statement ExpressionStatement()
        {
            var expr = Expression();
            Consume(LINE_END, "Expected line end after value.");
            return new Statement.ExpressionStatement(expr);
        }
        
        // Expressions
        private Expr Expression()
        {
            return Assignment();
        }

        private Expr Assignment()
        {
            var expr = Or();

            if (Match(EQUAL))
            {
                var equals = Previous();
                var val = Assignment();

                if (expr is Expr.Variable variable)
                    return new Expr.Assign(variable.Name, val);

                Error(equals, "Invalid assignment target.");
            }

            return expr;
        }

        private Expr Or()
        {
            var expr = And();

            while (Match(OR))
            {
                var op = Previous();
                var right = And();
                expr = new Expr.Logical(expr, op, right);
            }

            return expr;
        }

        private Expr And()
        {
            var expr = Equality();

            while (Match(AND))
            {
                var op = Previous();
                var right = Equality();
                expr = new Expr.Logical(expr, op, right);
            }

            return expr;
        }

        private Expr Equality()
        {
            var expr = Comparison();

            while (Match(EQUAL_EQUAL, BANG_EQUAL))
            {
                var op = Previous();
                var right = Comparison();
                expr = new Expr.Binary(expr, op, right);
            }

            return expr;
        }

        private Expr Comparison()
        {
            var expr = Term();

            while (Match(GREATER, GREATER_EQUAL, LESS, LESS_EQUAL))
            {
                var op = Previous();
                var right = Term();
                expr = new Expr.Binary(expr, op, right);
            }

            return expr;
        }

        private Expr Term()
        {
            var expr = Factor();

            while (Match(PLUS, MINUS, TILDE))
            {
                var op = Previous();
                var right = Factor();
                expr = new Expr.Binary(expr, op, right);
            }

            return expr;
        }

        private Expr Factor()
        {
            var expr = Unary();

            while (Match(STAR, SLASH))
            {
                var op = Previous();
                var right = Unary();
                expr = new Expr.Binary(expr, op, right);
            }

            return expr;
        }

        private Expr Unary()
        {
            if (Match(NOT, MINUS))
            {
                var op = Previous();
                var right = Unary();
                return new Expr.Unary(op, right);
            }

            return Call();
        }

        private Expr Call()
        {
            var expr = Primary();

            while (true)
            {
                if (Match(LEFT_PAREN))
                    expr = FinishCall(expr);
                else
                    break;
            }

            return expr;
        }

        private Expr FinishCall(Expr callee)
        {
            var args = new List<Expr>();

            if (!Check(RIGHT_PAREN))
            {
                do
                {
                    if (args.Count >= 255)
                        Error(Peek(), "A call can't have more than 255 arguments.");
                    args.Add(Expression());
                } while (Match(COMMA));
            }

            var paren = Consume(RIGHT_PAREN, "Expect ')' after arguments");

            return new Expr.Call(callee, paren, args);
        }

        private Expr Primary()
        {
            if (Match(FALSE))
                return new Expr.Literal(false);
            if (Match(TRUE))
                return new Expr.Literal(true);
            if (Match(NULL))
                return new Expr.Literal(null);

            if (Match(NUMBER, STRING))
                return new Expr.Literal(Previous().Literal);

            if (Match(IDENTIFIER))
                return new Expr.Variable(Previous());

            if (Match(LEFT_PAREN))
            {
                var expr = Expression();

                Consume(RIGHT_PAREN, "Expected ')' after expression.");
                return new Expr.Grouping(expr);
            }

            throw Error(Peek(), "Unexpected token.");
        }

        private void Synchronize()
        {
            Advance();

            while (!IsAtEnd())
            {
                if (Previous().Type == LINE_END) return;
                switch (Peek().Type)
                {
                    case CLASS:
                    case PROC:
                    case LET:
                    case RETURN:
                    case IF:
                    case WHILE:
                    case REPEAT:
                        return;
                    
                }

                Advance();
            }
        }

        private ParseError Error(Token token, string message)
        {
            Cipl.Error(token, message);
            return new ParseError();
        }
    }
}