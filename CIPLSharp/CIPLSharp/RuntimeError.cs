using System;

namespace CIPLSharp
{
    public class RuntimeError : Exception
    {
        public readonly Token Token;

        public RuntimeError(Token token, string message) : base("Error: " + message)
        {
            Token = token;
        }
    }
}