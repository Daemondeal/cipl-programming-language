using System;
using System.Collections.Generic;

namespace CIPLSharp.Runtime
{
    public class GetMethod : ICiplBindable
    {
        private VectorInstance vectorInstance;

        public int Arity() => 1;

        public object Call(Interpreter interpreter, List<object> arguments)
        {
            return vectorInstance.Get(Convert.ToInt32((double)arguments[0]));
        }

        public ICiplBindable Bind(CiplInstance instance)
        {
            vectorInstance = (VectorInstance)instance;
            return this;
        }
    }
}