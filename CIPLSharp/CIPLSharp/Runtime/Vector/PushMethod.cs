using System.Collections.Generic;

namespace CIPLSharp.Runtime
{
    public class PushMethod : ICiplBindable
    {
        private VectorInstance vectorInstance;
        
        public int Arity()
        {
            return 1;
        }

        public object Call(Interpreter interpreter, List<object> arguments)
        {
            vectorInstance.Push(arguments[0]);
            return null;
        }

        public ICiplBindable Bind(CiplInstance instance)
        {
            vectorInstance = (VectorInstance)instance;
            return this;
        }
    }
}