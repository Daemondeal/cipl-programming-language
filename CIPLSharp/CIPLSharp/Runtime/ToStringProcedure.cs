using System;
using System.Collections.Generic;

namespace CIPLSharp.Runtime
{
    public class ToStringProcedure : NativeProcedure
    {
        public override int Arity() => 1;
        
        public override object Call(Interpreter interpreter, List<object> arguments)
        {
            return Interpreter.Stringify(arguments[0]);
        }
    }
}