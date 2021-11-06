using System.Collections.Generic;

namespace CIPLSharp
{
    public class CiplProcedure : ICiplCallable
    {
        private readonly Statement.Procedure declaration;
        private readonly Environment closure;

        public CiplProcedure(Statement.Procedure declaration, Environment closure)
        {
            this.declaration = declaration;
            this.closure = closure;
        }
        
        public int Arity() => declaration.Parameters.Count;

        public object Call(Interpreter interpreter, List<object> arguments)
        {
            var environment = new Environment(closure);

            for (var i = 0; i < declaration.Parameters.Count; i++)
                environment.Define(declaration.Parameters[i].Lexeme, arguments[i]);

            try
            {
                interpreter.ExecuteBlock(declaration.Body, environment);
            } catch (Return returnValue)
            {
                return returnValue.Value;
            }
            return null;
        }

        public override string ToString()
        {
            return "<proc " + declaration.Name.Lexeme + ">";
        }
    }
}