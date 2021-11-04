using System;
using System.Collections.Generic;

namespace CIPLSharp.Runtime
{
    public class PrintProcedure : NativeProcedure
    {
        public override int Arity() => 1;

        public override object Call(Interpreter interpreter, List<object> arguments)
        {
            Console.WriteLine(Interpreter.Stringify(arguments[0]));
            return null;
        }
    }
}