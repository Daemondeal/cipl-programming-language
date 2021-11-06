using System;
using System.Collections.Generic;

namespace CIPLSharp.Runtime
{
    public class WaitProcedure : NativeProcedure
    {
        public override int Arity() => 1;

        public override object Call(Interpreter interpreter, List<object> arguments)
        {
            var ms = Convert.ToInt32((double) arguments[0] * 1000);
            
            System.Threading.Thread.Sleep(ms);
            return null;
        }
    }
}