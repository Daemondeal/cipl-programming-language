using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml;
using static CIPLSharp.TokenType;

namespace CIPLSharp
{
    public class Scanner
    {
        private readonly string source;
        private readonly List<Token> tokens = new List<Token>();
        private readonly Stack<int> indentationStack = new Stack<int>();
        
        private static readonly Dictionary<string, TokenType> Keywords = new Dictionary<string, TokenType>()
        {
            { "true", TRUE }, 
            { "false", FALSE }, 
            { "null", NULL }, 
            { "pass", PASS },
            { "not", NOT }, 
            { "and", AND }, 
            { "or", OR }, 
            { "let", LET }, 
            { "if", IF }, 
            { "else", ELSE }, 
            { "while", WHILE }, 
            { "repeat", REPEAT }, 
            { "for", FOR },
            { "in", IN },
            { "break", BREAK },
            { "proc", PROC }, 
            { "return", RETURN },
            { "class", CLASS }, 
            { "super", SUPER }, 
            { "this", THIS },
        };

        private int start;
        private int current;
        private int line = 1;

        public Scanner(string source)
        {
            this.source = source;
        }

        // Utils
        private bool IsAtEnd()
        {
            return current >= source.Length;
        }
        private bool IsAlphanumeric(char c)
        {
            return char.IsDigit(c) || char.IsLetter(c) || c == '_';
        }

        private bool IsWhitespace(char c)
        {
            return c is ' ' or '\t';
        }
        
        // Navigating the source
        private char Advance()
        {
            return source[current++];
        }

        private char Peek()
        {
            return IsAtEnd() ? '\0' : source[current];
        }

        private char PeekNext()
        {
            return current + 1 >= source.Length ? '\0' : source[current + 1];
        }

        private char PeekNextIgnoringWhitespace()
        {
            for (var i = current + 1; i < source.Length; i++)
            {
                if (!char.IsWhiteSpace(source[i]))
                    return source[i];
            }

            return '\0';
        }

        private bool IsNextLineEmpty()
        {
            for (var i = current; i < source.Length; i++)
            {
                if (source[i] == '\n') return true;
                if (!IsWhitespace(source[i])) return false;
            }

            return false;
        }

        private int CountWhitespaces()
        {
            var count = 0;

            while (current + count < source.Length && IsWhitespace(source[current + count]))
                count++;

            return count;
        }

        private bool Match(char expected)
        {
            if (IsAtEnd()) return false;
            if (source[current] != expected) return false;

            current++;
            return true;
        }

        private Token LastToken()
        {
            return tokens[^1];
        }
        
        // Adding stuff
        private void AddToken(TokenType type, object literal = null)
        {
            var text = source.Substring(start, current - start);
            tokens.Add(new Token(type, text, literal, line));
        }

        private void AddEmptyToken(TokenType type)
        {
            tokens.Add(new Token(type, "", null, line));
        }

        private void AddString()
        {
            while (Peek() != '"' && !IsAtEnd())
            {
                if (Peek() == '\n') line++;
                Advance();
            }
            
            // EOF reached before string ends
            if (IsAtEnd())
            {
                Cipl.Error(line, "End of file reached before a string terminates.");
                return;
            }

            // Consume the closing '"'
            Advance();

            var value = source.Substring(start + 1, current - start - 2);
            AddToken(STRING, value);
        }

        private void AddNumber()
        {
            while (char.IsDigit(Peek())) Advance(); // Consume the number

            if (Peek() == '.' && char.IsDigit(PeekNext()))
            {
                Advance();

                while (char.IsDigit(Peek())) Advance();
            }

            var value = double.Parse(source.Substring(start, current - start));
            AddToken(NUMBER, value);
        }

        private void AddIdentifier()
        {
            while (IsAlphanumeric(Peek())) Advance();

            var text = source.Substring(start, current - start);

            AddToken(Keywords.TryGetValue(text, out var type) ? type : IDENTIFIER);
        }

        private void ScanToken()
        {
            var c = Advance();

            switch (c)
            {
                case '(': 
                    AddToken(LEFT_PAREN);
                    break;
                case ')':
                    AddToken(RIGHT_PAREN);
                    break;
                case ',':
                    AddToken(COMMA);
                    break;
                case '.':
                    AddToken(Match('.') ? Match('.') ? THREE_DOTS : DOT_DOT : DOT);
                    break;
                case '-':
                    AddToken(MINUS);
                    break;
                case '+':
                    AddToken(PLUS);
                    break;
                case '*':
                    AddToken(STAR);
                    break;
                case '/':
                    AddToken(SLASH);
                    break;
                case '~':
                    AddToken(TILDE);
                    break;
                case ':':
                    AddToken(COLON);
                    break;
                case '!':
                    if (Match('='))
                        AddToken(BANG_EQUAL);
                    else
                        Cipl.Error(line, $"Unexpected character '{c}'");
                    
                    break;
                case '=':
                    AddToken(Match('=') ? EQUAL_EQUAL : EQUAL);
                    break;
                case '<':
                    AddToken(Match('=') ? LESS_EQUAL : LESS);
                    break;
                case '>':
                    AddToken(Match('=') ? GREATER_EQUAL : GREATER);
                    break;
                case '#':
                    // A comment goes until the end of the line.
                    while (Peek() != '\n' && !IsAtEnd()) Advance();
                    
                    break;

                case ' ':
                case '\r':
                case '\t':
                    break;
                
                case '\n':
                    line++;
                    if (PeekNextIgnoringWhitespace() == '.' || tokens.Count == 0)
                        break;
                    if (LastToken().Type != LINE_END && LastToken().Type != DEDENT)
                        AddToken(LINE_END);
                    
                    // Indentation management
                    while (IsNextLineEmpty())
                    {
                        while (Peek() != '\n' && !IsAtEnd()) Advance();
                        Advance();
                        line++;
                    }

                    if (!IsAtEnd())
                    {
                        var indentLevel = CountWhitespaces();

                        if (indentLevel > indentationStack.Peek())
                        {
                            indentationStack.Push(indentLevel);
                            AddEmptyToken(INDENT);
                        }
                        else
                        {
                            while (indentLevel < indentationStack.Peek())
                            {
                                indentationStack.Pop();
                                AddEmptyToken(DEDENT);
                            }
                        }
                    }


                    break;
                
                case '"':
                    AddString();
                    break;
                
                default:
                    if (char.IsDigit(c))
                        AddNumber();
                    else if (char.IsLetter(c))
                        AddIdentifier();
                    else
                        Cipl.Error(line, $"Unexpected character '{c}'");
                    
                    break;
            }
            
        }

        public List<Token> ScanTokens()
        {
            indentationStack.Push(0);
            if (source.StartsWith(' ') || source.StartsWith('\t'))
            {
                Cipl.Error(0, "Source file can't start with an indented block");
            }
            while (!IsAtEnd())
            {
                start = current;
                ScanToken();
            }

            if (tokens.Count > 0 && LastToken().Type != LINE_END)
                tokens.Add(new Token(LINE_END, "\n", null, line));
            
            while (indentationStack.Peek() != 0)
            {
                indentationStack.Pop();
                AddEmptyToken(DEDENT);
            }
            
            tokens.Add(new Token(EOF, "", null, line));
            return tokens;
        }
    }
}