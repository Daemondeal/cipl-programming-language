using System.Collections.Generic;

namespace CIPLSharp.Runtime
{
    public class LengthMethod : ICiplBindable
    {
        private VectorInstance vectorInstance;

        public int Arity() => 0;

        public object Call(Interpreter interpreter, List<object> arguments)
        {
            return (double)vectorInstance.Length();
        }

        public ICiplBindable Bind(CiplInstance instance)
        {
            vectorInstance = (VectorInstance)instance;
            return this;
        }
    }
}