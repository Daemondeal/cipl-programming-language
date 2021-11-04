using System.Text;
using System.Text.RegularExpressions;

namespace CIPLSharp
{
    public readonly struct Token
    {
        public readonly TokenType Type;
        public readonly string Lexeme;
        public readonly object Literal;
        public readonly int Line;
        
        
        public Token(TokenType type, string lexeme, object literal, int line)
        {
            Type = type;
            Lexeme = lexeme;
            Literal = literal;
            Line = line;
        }

        public static string Escape(string s)
        {
            return s.Replace("\\", "\\\\")
                .Replace("\t", "\\t")
                .Replace("\b", "\\b")
                .Replace("\n", "\\n")
                .Replace("\r", "\\r")
                .Replace("\f", "\\f");
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(Type.ToString().PadRight(15));
            sb.Append(Escape(Lexeme).PadRight(15));
            sb.Append(Line.ToString().PadRight(5));
            if (Literal != null)
                sb.Append(" " + Literal);

            return sb.ToString();
        }
    }
}