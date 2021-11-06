using System.Collections.Generic;

namespace CIPLSharp
{
    public class CiplProcedure : ICiplCallable
    {
        private readonly Statement.Procedure declaration;
        private readonly Environment closure;
        private readonly bool isInitializer;

        public CiplProcedure(Statement.Procedure declaration, Environment closure, bool isInitializer)
        {
            this.declaration = declaration;
            this.closure = closure;
            this.isInitializer = isInitializer;
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
                if (isInitializer)
                    return closure.GetAt(0, "this");
                return returnValue.Value;
            }

            if (isInitializer)
                return closure.GetAt(0, "this");
            return null;
        }

        public override string ToString()
        {
            return "<proc " + declaration.Name.Lexeme + ">";
        }

        public CiplProcedure Bind(CiplInstance instance)
        {
            var env = new Environment(closure);
            env.Define("this", instance);
            return new CiplProcedure(declaration, env, isInitializer);
        }
    }
}