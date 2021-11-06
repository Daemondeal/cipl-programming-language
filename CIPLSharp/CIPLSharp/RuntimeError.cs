using System;

namespace CIPLSharp
{
    public class RuntimeError : Exception
    {
        public readonly Token Token;

        public RuntimeError(string message) : base("Error: " + message)
        {
            Token = new Token(TokenType.NULL, "", null, 0); // TODO: this is shit
        }
        
        public RuntimeError(Token token, string message) : base("Error: " + message)
        {
            Token = token;
        }
    }
}