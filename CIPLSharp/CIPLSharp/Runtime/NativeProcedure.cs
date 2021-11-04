using System.Collections.Generic;

namespace CIPLSharp.Runtime
{
    public abstract class NativeProcedure : ICiplCallable
    {
        public abstract int Arity();
        public abstract object Call(Interpreter interpreter, List<object> arguments);

        public override string ToString()
        {
            return "<native proc>";
        }
    }
}