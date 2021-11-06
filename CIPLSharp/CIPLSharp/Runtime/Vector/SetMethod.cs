using System;
using System.Collections.Generic;

namespace CIPLSharp.Runtime
{
    public class SetMethod : ICiplBindable
    {
        private VectorInstance vectorInstance;
        
        public int Arity()
        {
            return 2;
        }

        public object Call(Interpreter interpreter, List<object> arguments)
        {
            var index = Convert.ToInt32((double) arguments[0]);
            
            vectorInstance.Set(index, arguments[1]);
            return null;
        }

        public ICiplBindable Bind(CiplInstance instance)
        {
            vectorInstance = (VectorInstance)instance;
            return this;
        }
    }
}